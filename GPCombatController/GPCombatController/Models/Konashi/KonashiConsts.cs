namespace GPCombatController.Models.Konashi
{
    public enum EKonashiUartBaudrate
    {
        UartRate9K6 = 0x0028,
        UartRate19K2 = 0x0050,
        UartRate38K4 = 0x00a0,
        UartRate57K6 = 0x00f0,
        UartRate76K8 = 0x0140,
        UartRate115K2 = 0x01e0,
    }

public class KonashiConsts
    {
        public const int Pio0 = 0;
        public const int Pio1 = 1;
        public const int Pio2 = 2;
        public const int Pio3 = 3;
        public const int Pio4 = 4;
        public const int Pio5 = 5;
        public const int Pio6 = 6;
        public const int Pio7 = 7;
        public const int PioCount = 8;

        public const int S1 = 0;

        public const int Led2 = 1;
        public const int Led3 = 2;
        public const int Led4 = 3;
        public const int Led5 = 4;

        public const int Aio1 = 1;
        public const int Aio2 = 2;

        public const int I2CSda = 6;
        public const int I2CScl = 7;

        public const int Hight = 1;
        public const int Low = 0;
        public const int True = 1;
        public const int False = 0;

        public const int PullUp = 1;
        public const int NoPulls = 0;

        public const int AnalogReference = 1300;

        //public const int PwmDisable = 0;
        //public const int PwmEnable = 1;
        public const int PwmEnableLedMode = 2;
        public const int PwmLedPeriod = 10000;

        //public const int UartDisable = 0;
        //public const int UartEnable = 1;
        public const int UartRate9K6 = 0x0028;
        public const int UartRate19K2 = 0x0050;
        public const int UartRate28K4 = 0x00a0;
        public const int UartRate57K6 = 0x00f0;
        public const int UartRate76K8 = 0x0140;
        public const int UartRate115K2 = 0x01e0;

        public const int UartDataMaxLength = 18;

        public const int I2CDatamaxLength = 19;
        //public const int I2CDisable = 0;
        //public const int I2CEnable = 1;
        public const int I2CEnable100K = 1;
        public const int I2CEnable400K = 2;
        public const int I2CStopCondition = 0;
        public const int I2CStartCondition = 1;
        public const int I2CRestartCondition = 2;
        public const int Success = 0;
        public const int Failure = -1;
    }
}
