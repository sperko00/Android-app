using System;

namespace Free_Room
{
    public class RecyclerViewEventArgs : EventArgs
    {
        private string ruid;

        public string RUID
        {
            get { return ruid; }
            set { ruid = value; }
        }

        public RecyclerViewEventArgs(string ruid) : base()
        {
            this.ruid = ruid;
        }
    }
}