using System.Collections.Generic;
using System.Linq;

namespace ALS.AntiPlagModule.Services.CompareModels
{
    public class CompareLCS: BaseCompare
    {
        public CompareLCS(ICollection<int> firstParam, ICollection<int> secondParam) : base(firstParam, secondParam) { }

        /// <summary>
        /// Calculating of LCS algorithm!
        /// </summary>
        /// <returns>LCS alg value</returns>
        public override int Execute()
        {
            int u = 0, v = 0;
            var arr1 = FirstParam.ToArray();
            var arr2 = SecondParam.ToArray();
            int len1 = arr1.Count();
            int len2 = arr2.Count();
            int res = 0;
            if (len1 != 0 && len2 != 0)
            {
                var a = new int[len1 + 1, len2 + 2];

                for (int i = 0; i < len1; ++i)
                {
                    for (int j = 0; j < len2; ++j)
                    {
                        if (arr1[i] == arr2[j])
                        {
                            a[i + 1, j + 1] = a[i, j] + 1;
                            if (a[i + 1, j + 1] > a[u, v])
                            {
                                u = i + 1;
                                v = j + 1;
                            }
                        }
                    }
                }
                res = a[u, v];
            }
            return res;
        }
    }
}
