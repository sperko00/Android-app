using Android.App;
using Android.Graphics;
using Android.OS;
using Android.Widget;
using XpandItComponents;

namespace TestRangeSlider
{
    [Activity(Label = "RangeSliderView Examples", MainLauncher = true, Icon = "@drawable/icon", Theme = "@android:style/Theme.Holo.Light")]
    public class MainActivity : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Main);
            var minTv = FindViewById<TextView>(Resource.Id.min_tv);
            var maxTv = FindViewById<TextView>(Resource.Id.max_tv);

            InitSlider1(minTv, maxTv);
            InitSlider2(minTv, maxTv);
            InitSlider3(minTv, maxTv);
            InitSlider4(minTv, maxTv);
            InitSlider5(minTv, maxTv);
            InitSlider6(minTv, maxTv);
        }

        private void InitSlider1(TextView minTv, TextView maxTv)
        {
            var rangeSlider = FindViewById<RangeSliderView>(Resource.Id.range_slider1);
            rangeSlider.RangeChanged += (sender, args) =>
            {
                minTv.Text = rangeSlider.SelectedMin.ToString("MIN: ##0");
                maxTv.Text = rangeSlider.SelectedMax.ToString("MAX: ##0");
            };
        }

        private void InitSlider2(TextView minTv, TextView maxTv)
        {
            var rangeSlider = FindViewById<RangeSliderView>(Resource.Id.range_slider2);
            rangeSlider.AbsoluteMin = 0;
            rangeSlider.AbsoluteMax = 1000;
            rangeSlider.ChangeThumbBitmaps(Resource.Drawable.normal_other_thumb, Resource.Drawable.pressed_other_thumb);
            rangeSlider.OverlayLineThinkness = 10;
            rangeSlider.OverlayLinePaint.Alpha = 255;
            rangeSlider.InsideLineThinkness = 5;
            rangeSlider.InsideLinePaint.Alpha = 100;
            rangeSlider.OverlayLinePaint.SetShader(new LinearGradient(0, 0, 0, (20), Color.Black, Color.White, Shader.TileMode.Mirror));
            rangeSlider.RangeChanged += (sender, args) =>
            {
                minTv.Text = rangeSlider.SelectedMin.ToString("MIN: ##0");
                maxTv.Text = rangeSlider.SelectedMax.ToString("MAX: ##0");
            };
        }

        private void InitSlider3(TextView minTv, TextView maxTv)
        {
            var rangeSlider = FindViewById<RangeSliderView>(Resource.Id.range_slider3);
            rangeSlider.ChangeThumbBitmaps(Resource.Drawable.normal_other_thumb_2, Resource.Drawable.pressed_other_thumb_2);
            rangeSlider.OverlayLinePaint.Color = Color.Gray;
            rangeSlider.ThumbsPaint.AntiAlias = false;
            rangeSlider.OverlayLineThinkness = (25);
            rangeSlider.OverlayLinePaint.Alpha = 100;
            rangeSlider.InsideLineThinkness = 0;
            rangeSlider.InsideLinePaint.Alpha = 100;
            rangeSlider.RangeChanged += (sender, args) =>
            {
                minTv.Text = rangeSlider.SelectedMin.ToString("MIN: ##0");
                maxTv.Text = rangeSlider.SelectedMax.ToString("MAX: ##0");
            };
        }

        private void InitSlider4(TextView minTv, TextView maxTv)
        {
            var rangeSlider = FindViewById<RangeSliderView>(Resource.Id.range_slider4);
            rangeSlider.ChangeThumbBitmaps(Resource.Drawable.normal_other_thumb_3, Resource.Drawable.pressed_other_thumb_3);
            rangeSlider.OverlayLineThinkness = (2);
            rangeSlider.OverlayLinePaint.Alpha = 100;
            rangeSlider.OverlayLinePaint.Color = Color.AliceBlue;
            rangeSlider.RangeChanged += (sender, args) =>
            {
                minTv.Text = rangeSlider.SelectedMin.ToString("MIN: ##0");
                maxTv.Text = rangeSlider.SelectedMax.ToString("MAX: ##0");
            };
        }

        private void InitSlider5(TextView minTv, TextView maxTv)
        {
            var rangeSlider = FindViewById<RangeSliderView>(Resource.Id.range_slider5);
            rangeSlider.ChangeThumbBitmaps(Resource.Drawable.normal_other_thumb_0, Resource.Drawable.pressed_other_thumb_0);
            rangeSlider.OverlayLineThinkness = (1);
            rangeSlider.OverlayLinePaint.Alpha = 50;
            rangeSlider.InsideLineThinkness = (1);
            rangeSlider.InsideLinePaint.Alpha = 100;
            rangeSlider.InsideLinePaint.Color = Color.LightGray;
            rangeSlider.OverlayLinePaint.Color = Color.CadetBlue;
            rangeSlider.RangeChanged += (sender, args) =>
            {
                minTv.Text = rangeSlider.SelectedMin.ToString("MIN: ##0");
                maxTv.Text = rangeSlider.SelectedMax.ToString("MAX: ##0");
            };
        }

        private void InitSlider6(TextView minTv, TextView maxTv)
        {
            var rangeSlider = FindViewById<RangeSliderView>(Resource.Id.range_slider6);
            rangeSlider.ChangeThumbBitmaps(Resource.Drawable.normal_other_thumb_1, Resource.Drawable.pressed_other_thumb_1);
            rangeSlider.OverlayLineThinkness = (15);
            rangeSlider.OverlayLinePaint.Alpha = 50;
            rangeSlider.OverlayLinePaint.Color = Color.Gray;
            rangeSlider.InsideLineThinkness = (5);
            rangeSlider.InsideLinePaint.Alpha = 100;
            rangeSlider.InsideLinePaint.Color = Color.LightGray;
            rangeSlider.RangeChanged += (sender, args) =>
            {
                minTv.Text = rangeSlider.SelectedMin.ToString("MIN: ##0");
                maxTv.Text = rangeSlider.SelectedMax.ToString("MAX: ##0");
            };
        }
    }
}

