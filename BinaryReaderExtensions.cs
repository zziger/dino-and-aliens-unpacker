using System.IO;
using System.Text;

static class BinaryReaderExtensions
{
    public static string ReadSZString(this BinaryReader reader)
    {
        var result = new StringBuilder();
        while (true)
        {
            byte b = reader.ReadByte();
            if (0 == b)
                break;
            result.Append((char)b);
        }
        return result.ToString();
    }
}