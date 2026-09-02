using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using DSMusic.Core;

namespace DSMusic.Forms;

/// <summary>
/// VuMeterSampler のデータを GDI+ で描画するコントロール。
/// スペクトラムバー（中央）＋ L/R VU バー（両端）の構成。
/// </summary>
public sealed class VuMeterPanel : Control
{
    private VuMeterSampler? _sampler;

    // 色設定
    private static readonly Color ColorLow    = Color.FromArgb(40, 200, 80);   // 緑
    private static readonly Color ColorMid    = Color.FromArgb(220, 200, 20);  // 黄
    private static readonly Color ColorHigh   = Color.FromArgb(220, 50, 40);   // 赤
    private static readonly Color ColorBg     = Color.FromArgb(24, 24, 24);
    private static readonly Color ColorHold   = Color.FromArgb(255, 140, 0);   // オレンジ（ピーク保持）
    private static readonly Color ColorBorder = Color.FromArgb(60, 60, 60);

    // ダブルバッファ用オフスクリーン
    private Bitmap? _backBuffer;

    public VuMeterPanel()
    {
        SetStyle(ControlStyles.OptimizedDoubleBuffer |
                 ControlStyles.AllPaintingInWmPaint  |
                 ControlStyles.UserPaint, true);
        BackColor = ColorBg;
        MinimumSize = new Size(200, 60);
    }

    public void SetSampler(VuMeterSampler? sampler)
    {
        _sampler = sampler;
        Invalidate();
    }

    /// <summary>外部タイマーから呼び出して再描画を要求する。</summary>
    public void Tick()
    {
        if (!IsHandleCreated || IsDisposed) return;
        Invalidate();
    }

    protected override void OnResize(EventArgs e)
    {
        base.OnResize(e);
        _backBuffer?.Dispose();
        _backBuffer = null;
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        int w = ClientSize.Width;
        int h = ClientSize.Height;
        if (w <= 0 || h <= 0) return;

        // オフスクリーンバッファ
        if (_backBuffer == null || _backBuffer.Width != w || _backBuffer.Height != h)
        {
            _backBuffer?.Dispose();
            _backBuffer = new Bitmap(w, h);
        }

        using var g = Graphics.FromImage(_backBuffer);
        g.SmoothingMode = SmoothingMode.None;
        g.Clear(ColorBg);

        if (_sampler == null)
        {
            // サンプラー未接続 → 枠だけ描画
            using var pen = new Pen(ColorBorder);
            g.DrawRectangle(pen, 0, 0, w - 1, h - 1);
            e.Graphics.DrawImage(_backBuffer, 0, 0);
            return;
        }

        const int vuW  = 14;  // L/R VU バーの幅
        const int gap  = 4;   // VU バーとスペクトラムの隙間

        int specX = vuW + gap;
        int specW = w - (vuW + gap) * 2;

        // ─── スペクトラムバー（中央）───
        int barCount = VuMeterSampler.BarCount;
        float barW = (float)specW / barCount;
        float[] bars = _sampler.Bars;

        for (int i = 0; i < barCount; i++)
        {
            float val = Math.Min(bars[i] * 3.5f, 1.0f); // 増幅して見やすく
            int barH = (int)(val * (h - 2));
            int x = specX + (int)(i * barW);
            int bw = Math.Max(1, (int)barW - 1);

            // 色をグラデーション（低=緑 中=黄 高=赤）
            Color c = val < 0.6f ? Lerp(ColorLow, ColorMid, val / 0.6f)
                                 : Lerp(ColorMid, ColorHigh, (val - 0.6f) / 0.4f);

            using var br = new SolidBrush(c);
            g.FillRectangle(br, x, h - 1 - barH, bw, barH);
        }

        // ─── L VU バー（左端）───
        DrawVuBar(g, 0, vuW, h, _sampler.PeakL, _sampler.HoldL);

        // ─── R VU バー（右端）───
        DrawVuBar(g, w - vuW, vuW, h, _sampler.PeakR, _sampler.HoldR);

        // ─── 外枠 ───
        using (var pen = new Pen(ColorBorder))
            g.DrawRectangle(pen, 0, 0, w - 1, h - 1);

        e.Graphics.DrawImage(_backBuffer, 0, 0);
    }

    private static void DrawVuBar(Graphics g, int x, int barW, int h, float peak, float hold)
    {
        float clamped = Math.Min(peak * 2.5f, 1.0f);
        int barH = (int)(clamped * (h - 4));
        int top  = h - 2 - barH;

        Color c = clamped < 0.6f ? Lerp(ColorLow, ColorMid, clamped / 0.6f)
                                 : Lerp(ColorMid, ColorHigh, (clamped - 0.6f) / 0.4f);

        using (var br = new SolidBrush(c))
            g.FillRectangle(br, x + 1, top, barW - 2, barH);

        // ピーク保持ライン
        float holdClamped = Math.Min(hold * 2.5f, 1.0f);
        int holdY = h - 2 - (int)(holdClamped * (h - 4));
        using (var pen = new Pen(ColorHold, 2f))
            g.DrawLine(pen, x + 1, holdY, x + barW - 2, holdY);
    }

    private static Color Lerp(Color a, Color b, float t)
    {
        t = Math.Clamp(t, 0f, 1f);
        return Color.FromArgb(
            (int)(a.R + (b.R - a.R) * t),
            (int)(a.G + (b.G - a.G) * t),
            (int)(a.B + (b.B - a.B) * t));
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing) _backBuffer?.Dispose();
        base.Dispose(disposing);
    }
}
