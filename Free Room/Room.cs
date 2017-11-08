namespace Free_Room
{
    public class Room
    {
        private string ime, adresa, ruid;
        private double cijena;

        public static int NoR = 0;
        public string Ime
        {
            get { return ime; } set { ime = value; }
        }
        public string RUID
        {
            get { return ruid; } set { ruid = value; }
        }
        public string Adresa
        {
            get { return adresa; } set { adresa = value; }
        }
        public double Cijena
        {
            get { return cijena; } set { cijena = value; }
        }
        public Room()
        {
            ime = "Ime sobe?";
            adresa = "Adresa sobe?";
            cijena = 0;
            ruid = "ERROR";
        }

        public Room(string ime, string adresa, double cijena, string ruid)
        {
            this.ime = ime;
            this.adresa = adresa;
            this.cijena = cijena;
            this.ruid = ruid;
        }
    }
}