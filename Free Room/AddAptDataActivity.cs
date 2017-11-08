using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using Android.Support.V7.App;
using SupportToolbar = Android.Support.V7.Widget.Toolbar;
using Free_Room.Model;
using Firebase.Xamarin.Database;
using Firebase.Auth;
using Android.Support.Design.Widget;
using System;
using Android.Content;
using Android.Runtime;
using Android.Graphics;
using Firebase.Storage;
using Firebase;
using Android.Gms.Tasks;
using Android.Gms.Location.Places.UI;
using Android.Gms.Maps.Model;
using Plugin.Connectivity;
using System.IO;

namespace Free_Room
{
    [Activity(Label = "AddAptDataActivity", Theme = "@style/AppTheme", MainLauncher = false)]
    public class AddAptDataActivity : AppCompatActivity, IOnProgressListener, IOnSuccessListener, IOnFailureListener
    {
        Button save, cancel, upload, set_location;
        SupportToolbar aptDataToolbar;
        EditText input_name, input_city, input_rooms, input_beds, input_person, input_price, input_description, input_contact;
        CheckBox check_wifi, check_klima, check_tv, check_parking;
        ImageView img_upload;
        FirebaseAuth auth;
        TextView adress_selected, progress_view;
        string image_id;
        private const string FirebaseURL = "https://freeroom-74739.firebaseio.com/";
        private const int PICK_IMAGE_REQUEST = 71;
        private const int PLACE_PICKER_REQUEST = 1;
        private Android.Net.Uri filePath;
        double picked_lat, picked_lng;
        public Marker aptLocationMarker;
        FirebaseStorage storage;
        StorageReference storageRef;
        private CoordinatorLayout addaptact;
        private ProgressBar progress_bar;
        bool main;
        
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            main = Intent.GetBooleanExtra("main", false);
            SetContentView(Resource.Layout.addAptData);
            //Init Firebase
            auth = FirebaseAuth.GetInstance(Login.app);
            if (auth != null)
            {
                FirebaseApp.InitializeApp(this);
                storage = FirebaseStorage.Instance;
                storageRef = storage.GetReferenceFromUrl("gs://freeroom-74739.appspot.com");
                aptDataToolbar = FindViewById<SupportToolbar>(Resource.Id.addAptData_toolbar);
                SetSupportActionBar(aptDataToolbar);
                Title = "Novi oglas";

                input_name = FindViewById<EditText>(Resource.Id.addAptData_ime);
                input_city = FindViewById<EditText>(Resource.Id.addAptData_grad);
                input_rooms = FindViewById<EditText>(Resource.Id.addAptData_brojSoba);
                input_person = FindViewById<EditText>(Resource.Id.addAptData_brojOsoba);
                input_beds = FindViewById<EditText>(Resource.Id.addAptData_brojKreveta);
                input_description = FindViewById<EditText>(Resource.Id.addAptData_opis);
                input_price = FindViewById<EditText>(Resource.Id.addAptDataCijena);
                check_wifi = FindViewById<CheckBox>(Resource.Id.addAptData_wifi);
                check_klima = FindViewById<CheckBox>(Resource.Id.addAptData_klima);
                check_tv = FindViewById<CheckBox>(Resource.Id.addAptData_tv);
                check_parking = FindViewById<CheckBox>(Resource.Id.addAptData_parking);
                cancel = FindViewById<Button>(Resource.Id.aptDataOdustani_button);
                save = FindViewById<Button>(Resource.Id.aptDataSpremi_button);
                upload = FindViewById<Button>(Resource.Id.aptDataPrenesi_button);
                set_location = FindViewById<Button>(Resource.Id.set_location);
                adress_selected = FindViewById<TextView>(Resource.Id.adress_selected);
                input_contact = FindViewById<EditText>(Resource.Id.addAptData_kontakt);
                img_upload = FindViewById<ImageView>(Resource.Id.img_upload);
                addaptact = FindViewById<CoordinatorLayout>(Resource.Id.activity_addaptdata);
                progress_bar = FindViewById<ProgressBar>(Resource.Id.progress_bar_addaptdata);
                progress_view = FindViewById<TextView>(Resource.Id.progress_text);
                save.Click += (o, e) =>
                {
                    if (input_name.Text != "")
                        if (input_city.Text != "")
                            if (input_beds.Text != "")
                                if (input_rooms.Text != "")
                                    if (input_person.Text != "")
                                        if (input_description.Text != "")
                                            if (input_price.Text != "")
                                                if (input_contact.Text != "")
                                                    if (adress_selected.Text != "")
                                                        if (img_upload.Drawable != null)
                                                        {
                                                            var connection = CrossConnectivity.Current.IsConnected;
                                                            if (connection)
                                                            {
                                                                UploadImage();
                                                                CreateApt();
                                                            }
                                                            else
                                                            {
                                                                Toast.MakeText(this, "Niste povezani na Internet!", ToastLength.Short).Show();
                                                            }
                                                        }
                                                        else
                                                            Toast.MakeText(this, "Niste odabrali sliku.", ToastLength.Short).Show();
                                                    else
                                                        Toast.MakeText(this, "Niste unijeli adresu.", ToastLength.Short).Show();
                                                else
                                                    Toast.MakeText(this, "Niste unijeli kontakt.", ToastLength.Short).Show();
                                            else
                                                Toast.MakeText(this, "Niste unijeli cijenu.", ToastLength.Short).Show();
                                        else
                                            Toast.MakeText(this, "Niste unijeli opis.", ToastLength.Short).Show();
                                    else
                                        Toast.MakeText(this, "Niste unijeli broj osoba.", ToastLength.Short).Show();
                                else
                                    Toast.MakeText(this, "Niste unijeli broj soba.", ToastLength.Short).Show();
                            else
                                Toast.MakeText(this, "Niste unijeli broj kreveta.", ToastLength.Short).Show();
                        else
                            Toast.MakeText(this, "Niste unijeli grad.", ToastLength.Short).Show();
                    else
                        Toast.MakeText(this, "Niste unijeli ime apartmana.", ToastLength.Short).Show();
                };

                cancel.Click += (o, e) =>
                {
                    Android.Support.V7.App.AlertDialog.Builder alert = new Android.Support.V7.App.AlertDialog.Builder(this);
                    alert.SetTitle("Potvrdite odustajanje");
                    alert.SetMessage("Da li želite odustati od unosa podataka?");
                    alert.SetPositiveButton("Da", (senderAlert, args) =>
                    {
                        bool main = Intent.GetBooleanExtra("main", false);
                        if (main == true)
                            StartActivity(typeof(MainActivity));
                        Finish();
                    });
                    alert.SetNegativeButton("Ne", (senderAlert, args) =>
                    {
                        Toast.MakeText(this, "OK", ToastLength.Short).Show();
                    });

                    Dialog dialog = alert.Create();
                    dialog.Show();
                };
                upload.Click += (o, e) =>
                {
                    ChooseImage();
                };

                set_location.Click += (o, e) =>
                {
                    PickAPlace();
                };
            }
            else
            {
                Toast.MakeText(this, "Odjavljeni ste, molimo prijavite se ponovno!", ToastLength.Short).Show();
                Finish();
                StartActivity(typeof(Login));
            }
        }

        private void PickAPlace()
        {
            var builder = new PlacePicker.IntentBuilder();
            StartActivityForResult(builder.Build(this), PLACE_PICKER_REQUEST);
        }

        private void ChooseImage()
        {
            Intent intent = new Intent();
            intent.SetType("image/*");
            intent.SetAction(Intent.ActionGetContent);
            StartActivityForResult(Intent.CreateChooser(intent, "Odaberite sliku"), PICK_IMAGE_REQUEST);

        }
        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if (requestCode == PICK_IMAGE_REQUEST && resultCode == Result.Ok && data != null && data.Data != null)
            {
                if (img_upload.GetDrawableState() != null && App.bitmap != null)
                {
                    img_upload.SetImageBitmap(null);
                    App.bitmap.Recycle();
                    App.bitmap.Dispose();
                    App.bitmap = null;

                }
                filePath = data.Data;
                int height = img_upload.Height;
                int width = img_upload.Width;

                DecodeBitmapFromStream(filePath, width, height);
               
                if (App.bitmap != null)
                {
                    img_upload.SetImageBitmap(App.bitmap);
                }
                GC.Collect();

            }


            if (requestCode == PLACE_PICKER_REQUEST && resultCode == Result.Ok)
            {
                GetPlaceFromPicker(data);
            }
            
        }

        private void DecodeBitmapFromStream(Android.Net.Uri data, int requestedWidth, int requestedHeight)
        {
            //Decode with InJustDecodeBounds = true to check dimensions
            Stream stream = ContentResolver.OpenInputStream(data);
            BitmapFactory.Options options = new BitmapFactory.Options();
            options.InJustDecodeBounds = true;
            BitmapFactory.DecodeStream(stream);

            //Calculate InSamplesize
            options.InSampleSize = CalculateInSampleSize(options, requestedWidth, requestedHeight);

            //Decode bitmap with InSampleSize set
            stream = ContentResolver.OpenInputStream(data); //Must read again
            options.InJustDecodeBounds = false;
            App.bitmap = BitmapFactory.DecodeStream(stream, null, options);
            
        }
        public static class App
        {
            public static Bitmap bitmap;
        }


        private int CalculateInSampleSize(BitmapFactory.Options options, int requestedWidth, int requestedHeight)
        {
            //Raw height and widht of image
            int height = options.OutHeight;
            int width = options.OutWidth;
            int inSampleSize = 1;

            if (height > requestedHeight || width > requestedWidth)
            {
                //the image is bigger than we want it to be
                int halfHeight = height / 2;
                int halfWidth = width / 2;

                while ((halfHeight / inSampleSize) > requestedHeight && (halfWidth / inSampleSize) > requestedWidth)
                {
                    inSampleSize *= 2;
                }

            }

            return inSampleSize;
        }


        private void GetPlaceFromPicker(Intent data)
        {
            var placePicked = PlacePicker.GetPlace(this, data);
            adress_selected.Text = placePicked?.AddressFormatted?.ToString();
            LatLng lokacija = placePicked.LatLng;
            picked_lat = lokacija.Latitude;
            picked_lng = lokacija.Longitude;
        }

        private void UploadImage()
        {
            if (auth != null)
            {
                var connection = CrossConnectivity.Current.IsConnected;
                if (connection)
                {
                    if (filePath != null)
                    {
                        progress_bar.Visibility = ViewStates.Visible;
                        addaptact.Visibility = ViewStates.Invisible;
                        progress_view.Visibility = ViewStates.Visible;
                        image_id = Guid.NewGuid().ToString();
                        var images = storageRef.Child("images/" + image_id);

                        images.PutFile(filePath)
                            .AddOnProgressListener(this)
                            .AddOnSuccessListener(this)
                            .AddOnFailureListener(this);
                    }
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
        private async void CreateApt()
        {

            AptInfo apt = new AptInfo();
            apt.aptUid = auth.CurrentUser.Uid;
            apt.aptName = input_name.Text;
            apt.aptCity = input_city.Text;
            apt.aptNumOfRooms = input_rooms.Text;
            apt.aptNumOfBeds = input_beds.Text;
            apt.aptDescription = input_description.Text;
            apt.aptPrice = input_price.Text;
            apt.aptNumOfPerson = input_person.Text;
            apt.aptContact = input_contact.Text;
            apt.aptSelectedAddress = adress_selected.Text;
            apt.aptLat = picked_lat;
            apt.aptLng = picked_lng;
            apt.aptImageId = image_id;

            if (check_wifi.Checked)
                apt.aptWifi = true;
            if (check_parking.Checked)
                apt.aptParking = true;
            if (check_klima.Checked)
                apt.aptKlima = true;
            if (check_tv.Checked)
                apt.aptTV = true;

            if (auth != null)
            {
                var connection = CrossConnectivity.Current.IsConnected;
                if (connection)
                {

                    var firebase = new FirebaseClient(FirebaseURL);
                    var item = await firebase.Child("apartments").PostAsync<AptInfo>(apt);
                    Toast.MakeText(this, "Apartman je uspješno postavljen", ToastLength.Short).Show();
                    StartActivity(new Intent(this, typeof(MainActivity)));
                    CleanMemory();
                    Finish();
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
        public void OnProgress(Java.Lang.Object snapshot)
        {
            var taskSnapShot = (UploadTask.TaskSnapshot)snapshot;
            double progress = (100.0 * taskSnapShot.BytesTransferred / taskSnapShot.TotalByteCount);

            progress_view.Text = Convert.ToString((int)progress) + " %";

        }

        public void OnSuccess(Java.Lang.Object result)
        {
            progress_view.Visibility = ViewStates.Invisible;
        }

        public void OnFailure(Java.Lang.Exception e)
        {
            progress_bar.Visibility = ViewStates.Invisible;
            progress_view.Visibility = ViewStates.Invisible;
            addaptact.Visibility = ViewStates.Visible;
            Toast.MakeText(this, "Prijenos slike nije uspio, pokušajte ponovo!", ToastLength.Short).Show();

        }
        public void CleanMemory()
        {
            if (img_upload.GetDrawableState() != null)
            {
                img_upload.SetImageBitmap(null);
            }
            if (App.bitmap != null)
            {
                App.bitmap.Recycle();
                App.bitmap.Dispose();
                App.bitmap = null;
            }
        }
        public override void OnBackPressed()
        {
            if (main == true)
                StartActivity(typeof(MainActivity));
            CleanMemory();
            Finish();
        }
       
    }
}