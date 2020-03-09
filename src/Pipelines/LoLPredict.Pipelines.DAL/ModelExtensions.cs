using System;
using System.Collections.Generic;
using System.Linq;
using LoLPredict.Database.Models;

namespace LoLPredict.Pipelines.DAL
{
    public static class ModelExtensions
    {
        public static string Number(this Patch patch)
        {
            return $"{patch.Major}.{patch.Minor}.{patch.Version}";
        } 


        public static IList<int> CreatePatchComponents(this string patchNumber)
        {
            return patchNumber.Split('.')
                .Select(_ => Convert.ToInt32(_))
                .ToList();
        }
    }
}
