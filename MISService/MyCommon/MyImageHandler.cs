namespace MyCommon
{
    public abstract class MyImageHandler
    {
        public abstract void Update(byte[] pic);
        public abstract byte[] GetImage();
    }

    public interface  IImageHandler
    {
        public void Update(byte[] pic);
        public byte[] GetImage();
    }
}
