using Android.App;
using Android.OS;
using Android.Views;
using SupportToolbar = Android.Support.V7.Widget.Toolbar;
using Android.Support.V7.App;
using Android.Support.Design.Widget;
using Android.Widget;
using Android.Content;
using Firebase.Storage;
using Firebase;
using Firebase.Auth;
using Android.Gms.Tasks;
using Android.Graphics;
using Android.Content.PM;
using Plugin.Connectivity;

namespace Free_Room
{
    [Activity(Label = "activity_accomodation_mainpage", Theme = "@style/AppTheme")]


    public class Activity_accomodation_mainpage : AppCompatActivity, IOnSuccessListener, IOnFailureListener
    {
        //mainpage
        public ImageView main_image;
        string name, person, room, bed, price, contact, city, adress, description, wifi, klima, parking, tv;
        public TextView main_name, main_person, main_room, main_bed, main_price, main_contact, main_city, main_adress, main_description, main_wifi, main_klima, main_parking, main_tv;
        ImageView apt_image;
        double lat, lng;
        FirebaseAuth auth;
        FirebaseStorage storage;
        StorageReference storageRef;
        string imageID;
        private Task downloadtask;
        ProgressBar progress_bar;
        Button start_nav, start_call;
        private bool main = false;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            name = Intent.GetStringExtra("name");
            bed = Intent.GetStringExtra("beds");
            room = Intent.GetStringExtra("rooms");
            person = Intent.GetStringExtra("person");
            price = Intent.GetStringExtra("price") + " kn";
            contact = Intent.GetStringExtra("contact");
            city = Intent.GetStringExtra("city");
            adress = Intent.GetStringExtra("adress");
            description = Intent.GetStringExtra("description");
            wifi = Intent.GetStringExtra("wifi");
            klima = Intent.GetStringExtra("klima");
            parking = Intent.GetStringExtra("parking");
            tv = Intent.GetStringExtra("tv");
            imageID = Intent.GetStringExtra("imageid");
            lat = Intent.GetDoubleExtra("lat", 0);
            lng = Intent.GetDoubleExtra("lng", 0);
            main = Intent.GetBooleanExtra("main", false);

            SetContentView(Resource.Layout.accomodation_mainpage);

            SupportToolbar toolBar = FindViewById<SupportToolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolBar);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetDisplayShowTitleEnabled(false);
            CollapsingToolbarLayout collapsingToolBar = FindViewById<CollapsingToolbarLayout>(Resource.Id.collapsing_toolbar);
            //AUTH
            //Init Firebase
            auth = FirebaseAuth.GetInstance(Login.app);
            if (auth != null)
            {
                var connection = CrossConnectivity.Current.IsConnected;
                if (connection)
                {
                    //STORAGE
                    FirebaseApp.InitializeApp(this);
                    storage = FirebaseStorage.Instance;
                    storageRef = storage.GetReferenceFromUrl("gs://freeroom-74739.appspot.com");
                    start_nav = FindViewById<Button>(Resource.Id.start_navigation_button);
                    start_call = FindViewById<Button>(Resource.Id.start_call);
                    //main views
                    main_image = FindViewById<ImageView>(Resource.Id.apt_image_main);
                    main_name = FindViewById<TextView>(Resource.Id.apt_name_main);
                    main_person = FindViewById<TextView>(Resource.Id.num_person_main);
                    main_room = FindViewById<TextView>(Resource.Id.num_rooms_main);
                    main_bed = FindViewById<TextView>(Resource.Id.num_beds_main);
                    main_price = FindViewById<TextView>(Resource.Id.price_main);
                    main_contact = FindViewById<TextView>(Resource.Id.phone_main);
                    main_city = FindViewById<TextView>(Resource.Id.city_main);
                    main_adress = FindViewById<TextView>(Resource.Id.adress_main);
                    main_description = FindViewById<TextView>(Resource.Id.description_main);
                    main_wifi = FindViewById<TextView>(Resource.Id.wifi_main);
                    main_klima = FindViewById<TextView>(Resource.Id.klima_main);
                    main_parking = FindViewById<TextView>(Resource.Id.parking_main);
                    main_tv = FindViewById<TextView>(Resource.Id.tv_main);
                    progress_bar = FindViewById<ProgressBar>(Resource.Id.progress);
                    progress_bar.Visibility = ViewStates.Visible;
                    apt_image = FindViewById<ImageView>(Resource.Id.apt_image_main);

                    main_name.Text = name;
                    main_bed.Text = bed;
                    main_room.Text = room;
                    main_person.Text = person;
                    main_price.Text = price;
                    main_contact.Text = contact;
                    main_city.Text = city;
                    main_adress.Text = adress;
                    main_description.Text = description;
                    main_wifi.Text = wifi;
                    main_klima.Text = klima;
                    main_parking.Text = parking;
                    main_tv.Text = tv;

                    start_nav.Click += (o, e) =>
                    {
                        if (isGoogleMapsInstalled())
                        {
                            var geoUri = Android.Net.Uri.Parse("google.navigation:q=" + lat + "," + lng);
                            var mapIntent = new Intent(Intent.ActionView, geoUri);
                            StartActivity(mapIntent);
                        }
                        else
                        {
                            Toast.MakeText(this, "Nemate google maps instaliran", ToastLength.Short).Show();
                        }

                    };
                    start_call.Click += (o, e) =>
                    {
                        var uri = Android.Net.Uri.Parse("tel:" + main_contact.Text);
                        var intent = new Intent(Intent.ActionDial, uri);
                        StartActivity(intent);
                    };
                    StorageReference image_ref = storageRef.Child("images/" + imageID);

                    downloadtask = image_ref.GetBytes(5000 * 5000);
                    downloadtask.AddOnSuccessListener(this);
                    downloadtask.AddOnFailureListener(this);
                }
                else
                {
                    Toast.MakeText(this, "Niste povezani na Internet!", ToastLength.Short).Show();
                }
            }
            else
            {
                Toast.MakeText(this, "Odjavljeni ste, molimo prijavite se ponovno!", ToastLength.Short).Show();
                Finish();
                StartActivity(typeof(Login));
            }

        }
        public bool isGoogleMapsInstalled()
        {
            try
            {
                ApplicationInfo info = PackageManager.GetApplicationInfo("com.google.android.apps.maps", 0);
                return true;
            }
            catch (PackageManager.NameNotFoundException e)
            {
                return false;
            }
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    {
                        if (main)
                            StartActivity(typeof(MainActivity));
                        CleanMemory();
                        Finish();
                    }
                    return true;
            }
            return base.OnOptionsItemSelected(item);
        }

        public void OnSuccess(Java.Lang.Object result)
        {
            if (downloadtask != null)
            {
                progress_bar.Visibility = ViewStates.Invisible;
                var data = downloadtask.Result.ToArray<byte>();
                App.bitmap = BitmapFactory.DecodeByteArray(data, 0, data.Length);
                apt_image.SetImageBitmap(App.bitmap);
                downloadtask = null;
            }
        }

        public void OnFailure(Java.Lang.Exception e)
        {
            Toast.MakeText(this, "Slika nije ucitana", ToastLength.Short).Show();
            progress_bar.Visibility = ViewStates.Invisible;
        }
        public override void OnBackPressed()
        {
            if (main)
                StartActivity(typeof(MainActivity));
            CleanMemory();
            
            Finish();
        }
        private void CleanMemory()
        {
            if (apt_image.GetDrawableState() != null)
            {
                apt_image.SetImageBitmap(null);
            }
            if (App.bitmap != null)
            {
                App.bitmap.Recycle();
                App.bitmap.Dispose();
                App.bitmap = null;
            }
        }
        public static class App
        {
            public static Bitmap bitmap;
        }

    }
}