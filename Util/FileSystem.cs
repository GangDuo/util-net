using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Util
{
    public class FileSystem
    {
        public static string GetRandomFileName(string prefix, string extension)
        {
            return prefix + Path.GetFileNameWithoutExtension(Path.GetRandomFileName()) + extension;
        }

        public static void MoveFolder(string source, string destination)
        {
            //[参照の追加]でMicrosoft Scripting Runtimeを事前追加すること
            Scripting.FileSystemObject fso = new Scripting.FileSystemObject();
            fso.MoveFolder(source, destination);
        }

        public static void MoveFile(string sourcePath, string destPath)
        {
            var fInfo = new FileInfo(sourcePath);
            fInfo.MoveTo(destPath);
        }

        /// <summary>
        /// バイナリファイル出力(ファイル全部を一括で書込む)
        /// </summary>
        /// <param name="fpath">ファイルPATH</param>
        /// <param name="dt">出力バイナリデータ</param>
        public static void WriteBinary(String fpath, Byte[] dt)
        {
            WriteBinary(fpath, (bw) =>
                {
                    bw.Write(dt);
                });
        }

        public static void WriteBinary(String fpath, Stream stream)
        {
            stream.Seek(0, SeekOrigin.Begin);
            WriteBinary(fpath, (bw) =>
                {
                    byte[] buf = new byte[1024];
                    int num;
                    while ((num = stream.Read(buf, 0, buf.Length)) > 0)
                    {
                        bw.Write(buf, 0, num);
                    }
                });
        }

        private static void WriteBinary(String fpath, Action<BinaryWriter> callback)
        {
            Stream sw = null;
            BinaryWriter bw = null;
            try
            {
                sw = File.Open(fpath, FileMode.Create, FileAccess.Write);
                bw = new BinaryWriter(sw);
                callback(bw);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (bw != null) bw.Close();
                if (sw != null) sw.Close();
            }
        }

        /// <summary>
        /// テキストファイル出力(ファイル全部を一括で書込む)
        /// </summary>
        /// <param name="fpath">ファイルPATH</param>
        /// <param name="cname">文字コード名</param>
        /// <param name="dt">出力文字列データ</param>
        /// ＜主な文字コード名＞
        /// SJIS="SHIFT-JIS"
        /// JIS="ISO-2022-JP"
        /// EUC="EUC-JP"
        /// UNICODE="UNICODE"
        /// UTF7="UTF-7"
        /// UTF8="UTF-8"
        public static void WriteText(String fpath, String cname, String dt)
        {
            StreamWriter sw = null;
            try
            {
                // BOM無しエンコーディング作成
                var enc = System.Text.Encoding.GetEncoding(cname);
                if (cname == "UTF-8")
                {
                    // BOM無し
                    enc = new System.Text.UTF8Encoding(false);
                }
                else if (cname == "UNICODE")
                {
                    // BOM無し(リトルエンディアン)
                    enc = new System.Text.UnicodeEncoding(false, false);
                }
                sw = new StreamWriter(fpath, false, enc);
                sw.Write(dt);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (sw != null) sw.Close();
            }
        }

        public static bool FileCompare(string file1, string file2)
        {
            int file1byte;
            int file2byte;
            FileStream fs1;
            FileStream fs2;

            // Determine if the same file was referenced two times.
            if (file1 == file2)
            {
                // Return true to indicate that the files are the same.
                return true;
            }

            // Open the two files.
            fs1 = new FileStream(file1, FileMode.Open);
            fs2 = new FileStream(file2, FileMode.Open);

            // Check the file sizes. If they are not the same, the files 
            // are not the same.
            if (fs1.Length != fs2.Length)
            {
                // Close the file
                fs1.Close();
                fs2.Close();

                // Return false to indicate files are different
                return false;
            }

            // Read and compare a byte from each file until either a
            // non-matching set of bytes is found or until the end of
            // file1 is reached.
            do
            {
                // Read one byte from each file.
                file1byte = fs1.ReadByte();
                file2byte = fs2.ReadByte();
            }
            while ((file1byte == file2byte) && (file1byte != -1));

            // Close the files.
            fs1.Close();
            fs2.Close();

            // Return the success of the comparison. "file1byte" is 
            // equal to "file2byte" at this point only if the files are 
            // the same.
            return ((file1byte - file2byte) == 0);
        }


#if false
        private class Cx
        {
            public Stream inputStream;
            public byte[] buffer;
            public MemoryStream memStream;
            //private byte[] cache = new byte[256000000];
        }
        public static void ReadBinaryAsync(String fpath, String cname, String dt)
        {
            const int BufferSize = 256;
            var c = new Cx()
            {
                inputStream = File.OpenRead(fpath),
                // 読み込み結果を格納するバッファ
                buffer = new Byte[BufferSize],
                memStream = new MemoryStream(4 * 1024)
            };

            // 読み込みの始点
            int offset = 0;
            // 読み込み完了時のコールバック
            AsyncCallback myCallBack = new AsyncCallback(OnCompletedRead);
            // 状態を取得するためのオブジェクト
            object state = c;
            //memStream.Seek(0, SeekOrigin.Begin);
            c.inputStream.BeginRead(c.buffer, offset, c.buffer.Length, myCallBack, state);
            return;
        }

        private static void OnCompletedRead(IAsyncResult result)
        {
            var state = (Cx)result.AsyncState;

            int bytesRead = state.inputStream.EndRead(result);
            if (bytesRead == 0)
            {
                String s = Encoding.UTF8.GetString(state.memStream.ToArray()); // 文字列に変換
                Console.WriteLine("buffer={0}", s);
                return;
            }
            if (bytesRead > 0)
            {
                String s = Encoding.UTF8.GetString(state.buffer, 0, bytesRead); // 文字列に変換
                Console.WriteLine("buffer={0}", s);
                state.memStream.Write(state.buffer, 0, bytesRead);
                //Console.WriteLine(Encoding.UTF8.GetString(state.memStream.ToArray()));

                var myCallBack = new AsyncCallback(OnCompletedRead);
                // 残りを再び非同期読み込み
                state.inputStream.BeginRead(state.buffer, 0, state.buffer.Length, myCallBack, state);
            }
        }
#endif

    }
}
