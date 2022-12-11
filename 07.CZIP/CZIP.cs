using Engine._98.Headers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine._07.CZIP
{
  public class CZIP : GENERIC_MGR<CZIP>
  {
    private IEnumerable<TOutput> Zip<T1, T2, TOutput>(IEnumerable<T1> _iterable_first, IEnumerable<T2> _iterable_second, Func<T1, T2, TOutput> _generator)
    {
      using (var iter_first = _iterable_first.GetEnumerator())
      using (var iter_second = _iterable_second.GetEnumerator())
        while (iter_first.MoveNext() && iter_second.MoveNext())
          yield return _generator(iter_first.Current, iter_second.Current);
    }
  }
}
/*
 private void test_1()
{
  double[] xValue = { 0, 1, 2, 3, 4, 5, 6, 4, 5, 6, 7, 8, 9, 0, 1, 2, 3, 7, 8, 9 };
  double[] yValue = { 0, 1, 2, 3, 4, 5, 3, 4, 5, 6, 6, 7, 8, 9, 0, 1, 2, 7, 8, 9 };

  List<Tuple<double, double>> list_tup_zip = new List<Tuple<double, double>>(Zip(xValue, yValue, (x, y) => Tuple.Create(x, y)));
  int i = 0;
  list_tup_zip.ForEach(element => Console.WriteLine($" {i} == {element.Item1} | {i++} == {element.Item2}"));


}
 */