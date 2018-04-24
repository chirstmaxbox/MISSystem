using SpecDomain.BLL.EstTitle;

namespace SpecDomain.BLL.Drawing
{
    public class FileUpload
    {
        public static string GetFilePath(int estRevID, string folderPath)
        {
            var est = new MyEstRev(estRevID);
            var p = new SpecProjectDetail( est.Value .JobID );
            string jobNumber = p.JobNumber ;
            folderPath += jobNumber + "\\";
            if ((!System.IO.Directory.Exists(folderPath)))
            {
                System.IO.Directory.CreateDirectory(folderPath);
            }
            return folderPath;
        }


        public static  string GetFileDescription(string s1)
        {
            string s = s1;
            try
            {
                int i =s1.LastIndexOf("\\") ;
                s = s.Remove(0, i + 1);
                i = s.LastIndexOf(".");
                s =MyCommon.MyString .Left(s, i);
            }
            catch (System.IO.IOException ex)
            {
            }
            return s;

        }



    }


}