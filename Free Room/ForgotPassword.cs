using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using Android.Support.V7.App;
using static Android.Views.View;
using Android.Gms.Tasks;
using Firebase.Auth;
using System.Net.Mail;
using Plugin.Connectivity;

namespace Free_Room
{
    [Activity(Label = "ForgotPassword", Theme = "@style/LoginTheme")]
    public class ForgotPassword : AppCompatActivity, IOnClickListener, IOnCompleteListener
    {
        private EditText input_email;
        private Button btnResetPass;
        private TextView btnBack;
        private RelativeLayout activity_forgot;
        FirebaseAuth auth;
        private ProgressBar progress_bar;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.ForgotPassword);

            //Init Firebase
            auth = FirebaseAuth.GetInstance(Login.app);
            //View
            input_email = FindViewById<EditText>(Resource.Id.forgot_email);
            btnResetPass = FindViewById<Button>(Resource.Id.forgot_btn_reset);
            btnBack = FindViewById<TextView>(Resource.Id.forgot_btn_back);
            activity_forgot = FindViewById<RelativeLayout>(Resource.Id.activity_forgot);
            progress_bar = FindViewById<ProgressBar>(Resource.Id.progress_bar_forgot);
            btnResetPass.SetOnClickListener(this);
            btnBack.SetOnClickListener(this);
        }
        private static bool isValidEmail(string EmailToCheck)
        {
            try
            {
                MailAddress mail = new MailAddress(EmailToCheck);
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
        public void OnClick(View v)
        {
            if (v.Id == Resource.Id.forgot_btn_back)
            {
                StartActivity(new Intent(this, typeof(Login)));
                Finish();
            }
            else if (v.Id == Resource.Id.forgot_btn_reset)
            {
                if (isValidEmail(input_email.Text))
                {
                    ResetPassword(input_email.Text);
                }
                else
                {
                    Toast.MakeText(this, "Email koji ste unijeli nije valjan!", ToastLength.Long).Show();
                }

            }
        }

        private void ResetPassword(string email)
        {
            var connection = CrossConnectivity.Current.IsConnected;
            if (connection)
            {
                auth.SendPasswordResetEmail(email)
                    .AddOnCompleteListener(this, this);
                progress_bar.Visibility = ViewStates.Visible;
                activity_forgot.Visibility = ViewStates.Invisible;
            }
            else
            {
                Toast.MakeText(this, "Niste povezani na Internet!", ToastLength.Short).Show();
            }

        }

            public void OnComplete(Task task)
        {
            if (task.IsSuccessful == false)
            {
                progress_bar.Visibility = ViewStates.Invisible;
                activity_forgot.Visibility = ViewStates.Visible;
                Toast.MakeText(this, "Dogodila se greška. Pokušajte ponovo!", ToastLength.Long).Show();


            }
            else
            {
                Toast.MakeText(this, "Link za resetiranje lozinke poslan je na email : " + input_email.Text, ToastLength.Long).Show();
                Finish();
                StartActivity(new Intent(this, typeof(Login)));

            }
        }
    }
}