# Getting Started with RangeSlider
Very flexible range slider fully customizable with just a few lines of code

##Acquiring a license key
RangeSlider is free to use but with limited functionality. To acquire your license key email us at <xamarin.components@xpand-it.com> with the subject "RangeSlider - Request Key". An email will be sent back to you containing your license key for RangeSlider.

##Usage

####You can create a RangeSlider very easily

#####XML
	<XpandItComponents.RangeSliderView
		android:id="@+id/range_slider"
		android:layout_width="match_parent"
		android:layout_height="wrap_content" />
		
#####Code
	var rangeSlider = new XpandItComponents.RangeSliderView(this);

#####Register the component
Before using the component do not forget to register it. The non registered version won't allow you to change the slider's maximum and minimum range values.

	rangeSlider.RegisterComponent("example@email.com","awesomecredential");

#####Change the thumbs of the slider by using the drawable id or bitmaps
	rangeSlider.ChangeThumbBitmaps(Resource.Drawable.normal_other_thumb, Resource.Drawable.pressed_other_thumb);
	// or
	rangeSlider.ChangeThumbBitmaps(normalThumbBitmap, pressedThumbBitmap);

#####Change other atributes as you want
	rangeSlider.AbsoluteMin = 0;
	rangeSlider.AbsoluteMax = 1000;
	rangeSlider.InsideLineThinkness = 5;
	rangeSlider.InsideLinePaint.Alpha = 100;
	rangeSlider.InsideLinePaint.Color = Color.LightGray;
	rangeSlider.OverlayLineThinkness = 10;
	rangeSlider.OverlayLinePaint.Alpha = 255;
	rangeSlider.OverlayLinePaint.SetShader(new LinearGradient(0, 0, 0, (20), Color.Black, Color.White, Shader.TileMode.Mirror));
	
#####Listen to changes on the slider
    var minTv = FindViewById<TextView>(Resource.Id.min_tv);
    var maxTv = FindViewById<TextView>(Resource.Id.max_tv);
	rangeSlider.RangeChanged += (sender, args) =>
			{
				minTv.Text = rangeSlider.SelectedMin.ToString("MIN: ##0");
				maxTv.Text = rangeSlider.SelectedMax.ToString("MAX: ##0");
			};



##Requirements

* Android API 15+