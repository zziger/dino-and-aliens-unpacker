using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DinoAndAliensUnpacker
{
    internal static class Program
    {
        static int Fail(string message)
        {
            Console.WriteLine(message);
            Console.WriteLine("Нажмите любую клавишу чтобы выйти... Press any key to exit...");
            Console.ReadKey();
            return 1;
        }

        private static void Xor(IList<byte> data, string key)
        {
            for (var i = 0; i < data.Count; i++)
            {
                data[i] ^= (byte) key[i % key.Length];
            }
        }

        public static int Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;

            if (args.Length < 1) return Fail("Для того чтобы запаковать папку или распаковать .dat - перетащите файл на DinoAndAliensUnpacker.exe\nIn order to pack folder, or unpack .dat - drag and drop the file on the DinoAndAliensUnpacker.exe");

            if (File.Exists(args[0]))
            {
                var basePath = Path.Combine(Path.GetDirectoryName(args[0]) ?? ".", Path.GetFileNameWithoutExtension(args[0]));

                using (var file = File.OpenRead(args[0]))
                using (var reader = new BinaryReader(file))
                {
                    while (reader.BaseStream.Position < reader.BaseStream.Length)
                    {
                        var name = reader.ReadSZString();
                        var len = reader.ReadInt32();
                        var data = reader.ReadBytes(len);

                        var path = Path.Combine(basePath, name);

                        Console.WriteLine($"Unpacking {name}...");
                        Xor(data, name);
                        File.WriteAllBytes(path, data);
                    }
                }
            }
            else if (Directory.Exists(args[0]))
            {
                var files = Directory.EnumerateFiles(args[0], "*.*", SearchOption.TopDirectoryOnly).ToArray();
                var datFile = Path.Combine(Path.GetDirectoryName(args[0]) ?? ".", Path.GetFileName(args[0]) + ".dat");
                if (File.Exists(datFile)) File.Delete(datFile);

                using (var stream = File.OpenWrite(datFile))
                using (var writer = new BinaryWriter(stream))
                {
                    foreach (var file in files)
                    {
                        var name = Path.GetFileName(file);
                        var data = File.ReadAllBytes(file);
                        var len = data.Length;

                        Xor(data, name);

                        Console.WriteLine($"Packing {name}...");
                        writer.Write(Encoding.UTF8.GetBytes(name));
                        writer.Write('\0');
                        writer.Write(len);
                        writer.Write(data);
                    }
                }
            }
            else
            {
                return Fail("Файл не найден\nFile not found");
            }

            Console.WriteLine("Готово! Done!");
            Console.WriteLine("Нажмите любую клавишу чтобы выйти... Press any key to exit...");
            Console.ReadKey();
            return 0;
        }
    }
}
