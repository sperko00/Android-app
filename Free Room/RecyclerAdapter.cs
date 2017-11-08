using System;
using System.Collections.Generic;
using Android.Content;
using Android.Views;
using Android.Widget;
using Android.Support.V7.Widget;
using Android.Animation;

namespace Free_Room
{
    class RecyclerAdapter : RecyclerView.Adapter
    {
        private List<Room> mRooms;
        private RecyclerView mRecyclerView;
        private Context mContext;
        //animacija
        private int mCurrentPosition = -1;

        public event EventHandler<RecyclerViewEventArgs> BrisanjeKliknuto;
        public event EventHandler<RecyclerViewEventArgs> PromjenaKliknuta;
        public event EventHandler<RecyclerViewEventArgs> PregledKliknuto;

        //Konstruktor RecyclerView adaptera
        public RecyclerAdapter(List<Room> rooms, RecyclerView recyclerView, Context context)
        {
            mRooms = rooms;
            mRecyclerView = recyclerView;
            mContext = context;
        }

        private class MyView : RecyclerView.ViewHolder
        {
            public View MainView_View { get; set; }
            public TextView Ime_TextView { get; set; }
            public TextView Cijena_TextView { get; set; }
            public Button Uredi_Button { get; set; }
            public Button Pregled_Button { get; set; }
            public ImageButton Izbrisi_ImageButton { get; set; }
            public MyView(View view) : base(view)
            {
                MainView_View = view;
            }
        }
        public override int GetItemViewType(int position)
        {
            return Resource.Layout.myRoomsItem;
        }
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View myRoomsItem = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.myRoomsItem, parent, false);
            TextView txtIme = myRoomsItem.FindViewById<TextView>(Resource.Id.myRoomsIme_TextView);
            TextView txtCijena = myRoomsItem.FindViewById<TextView>(Resource.Id.myRoomsCijena_TextView);
            Button btnUredi = myRoomsItem.FindViewById<Button>(Resource.Id.myRoomsModify_Button);
            Button btnPregled = myRoomsItem.FindViewById<Button>(Resource.Id.myRoomsPreview_Button);
            ImageButton imgbtnIzbrisi = myRoomsItem.FindViewById<ImageButton>(Resource.Id.myRoomsDelete_ImageButton);

            MyView view = new MyView(myRoomsItem)
            {
                Ime_TextView = txtIme,
                Cijena_TextView = txtCijena,
                Uredi_Button = btnUredi,
                Pregled_Button = btnPregled,
                Izbrisi_ImageButton = imgbtnIzbrisi
            };
            return view;
        }
        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            MyView myHolder = holder as MyView;

            myHolder.MainView_View.Click += (o, e) =>
            {
                position = holder.AdapterPosition;
            };

            myHolder.Uredi_Button.Click += (o, e) =>
            {
                position = holder.AdapterPosition;
                if (mRooms.Count > 0 && position < mRooms.Count && position >= 0)
                    PromjenaKliknuta.Invoke(this, new RecyclerViewEventArgs(mRooms[position].RUID));
            };

            myHolder.Pregled_Button.Click += (o, e) =>
            {
                position = holder.AdapterPosition;
                if (mRooms.Count > 0 && position < mRooms.Count && position >= 0)
                    PregledKliknuto.Invoke(this, new RecyclerViewEventArgs(mRooms[position].RUID));
            };

            myHolder.Izbrisi_ImageButton.Click += (o, e) =>
            {
                position = holder.AdapterPosition;
                if (mRooms.Count > 0 && position < mRooms.Count && position >= 0)
                    BrisanjeKliknuto.Invoke(this, new RecyclerViewEventArgs(mRooms[position].RUID));
            };

            myHolder.Ime_TextView.Text = mRooms[position].Ime;
            myHolder.Cijena_TextView.Text = mRooms[position].Cijena.ToString()+" kn";

            if (position > mCurrentPosition)
            {
                int currentAnim = Resource.Animation.slide_left_to_right;
                SetAnimation(myHolder.MainView_View, currentAnim);
                mCurrentPosition = position;
            }
        }
        public override int ItemCount
        {
            get { return mRooms.Count; }
        }
        private void SetAnimation(View view, int currentAnim)
        {
            Animator animator = AnimatorInflater.LoadAnimator(mContext, Resource.Animation.flip);
            animator.SetTarget(view);
            animator.Start();
        }
    }
}