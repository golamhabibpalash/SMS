namespace SMS.Entities
{
    public class ParamBusConfig:CommonProps
    {
        public int ParamSL { get; set; }
        public string ConfigName { get; set; }
        public string ParamValue { get; set; }
        public string Remarks { get; set; }
        public bool IsActive { get; set; }
    }
}

//1 Checkin Time Start
//2 Checkin Time Stop
//3 Checkout Time Start
//4 Checkout TIme Stop
//5 Break Time Start
//6 Break Time Stop