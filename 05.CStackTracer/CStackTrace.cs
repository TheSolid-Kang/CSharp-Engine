using Engine._98.Headers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine._05.CStackTracer
{
  public class CStackTracer : GENERIC_MGR<CStackTracer>
  {
    const int DEFAULT_CAP = 2048;
    const int DEFAULT_FRAME_DEPTH = 2;

    public void write_trace_info(string _note = "", int _depth = DEFAULT_FRAME_DEPTH)
    {
      string path = System.IO.Directory.GetCurrentDirectory() + @"\my_log_stack.txt";
      System.Text.StringBuilder str_buil = new System.Text.StringBuilder(DEFAULT_CAP);
      if (!System.IO.File.Exists(path))
        System.IO.File.WriteAllText(path, "");

      str_buil.Append($"Thread == {System.Threading.Thread.CurrentThread.ManagedThreadId}\n");
      if (!_note.Equals(string.Empty))
      {
        str_buil.Append("==note: ");
        str_buil.Append(_note + "\n");
      }
      str_buil.Append($"stack ({System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")})\n");
      str_buil.Append("{\n");
      for (int i = 1; i <= _depth; ++i)
      {
        str_buil.Append($"  current stack == {i} : ");
        str_buil.Append(new System.Diagnostics.StackTrace().GetFrame(i)?.GetMethod()?.ReflectedType?.Name);// 이전 Class명
        str_buil.Append(".");
        str_buil.Append(new System.Diagnostics.StackFrame(i, true).GetMethod().Name);// 이전 함수명 
        str_buil.Append("()\n");
      }
      str_buil.Append("}\n\n");

      System.IO.File.AppendAllText(path, str_buil.ToString());
    }
  };
}
