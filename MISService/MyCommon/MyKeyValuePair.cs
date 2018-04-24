namespace MyCommon
{

    public class MyValueTextPair
    {
        public string Value { get; set; }
        public string Text { get; set; }
    }

    public class MyKeyValuePair
    {
        public int Key { get; set; }
        public string Value { get; set; }
    }

    public class MyLongKeyValuePair
    {
        public long Key { get; set; }
        public string Value { get; set; }
    }

    public class MyKeyValueBool
    {
        public int Key { get; set; }
        public string Value1 { get; set; }
        public string Value2 { get; set; }

        public bool IsChecked { get; set; }
    }

    public class MyLongKeyValueBool
    {
        public long  Key { get; set; }
        public string Value1 { get; set; }
        public string Value2 { get; set; }
        public bool IsChecked { get; set; }
    }

    public class MyKeyValueTriple
    {
        public int Key { get; set; }
        public string Value1 { get; set; }
        public string Value2 { get; set; }
    }

    public class MyKeyValueQuad
    {
        public int Key { get; set; }
        public string Value1 { get; set; }
        public string Value2 { get; set; }
        public string Value3 { get; set; }
    }
    public class MyKeyValueHexa
    {
        public int Key { get; set; }
        public string Value1 { get; set; }
        public string Value2 { get; set; }
        public string Value3 { get; set; }
        public string Value4 { get; set; }
        public string Value5 { get; set; }
    }

    public class MyKeyDoublePair
    {
        public int Key { get; set; }
        public double  Value { get; set; }

    }

    public class MyKeyDoubleQuard
    {
        public int Key { get; set; }
        public double Value1 { get; set; }
        public double Value2 { get; set; }
        public double Value3 { get; set; }

    }
    //public class MyAutoCompleteResponse
    //{
    //    public string Label { get; set; }
    //    public string Value { get; set; }
    //    public string Selected { get; set; }
    //}
}