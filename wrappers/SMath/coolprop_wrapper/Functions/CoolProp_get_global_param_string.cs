﻿using System.Runtime.InteropServices;
using SMath.Manager;

namespace coolprop_wrapper.Functions
{
  class CoolProp_get_global_param_string : IFunction
  {
    // long get_global_param_string(const char *param, char *Output, int n);
    [DllImport(
      "CoolProp.x86.dll", EntryPoint = "get_global_param_string",
      CharSet = CharSet.Ansi)]
    internal static extern long CoolPropDLLfunc_x86(
      string param,
      System.Text.StringBuilder Output,
      int n);
    [DllImport(
      "CoolProp.x64.dll", EntryPoint = "get_global_param_string",
      CharSet = CharSet.Ansi)]
    internal static extern long CoolPropDLLfunc_x64(
      string param,
      System.Text.StringBuilder Output,
      int n);
    internal static bool CoolPropDLLfunc(string param, out string resultStr)
    {
      var output = new System.Text.StringBuilder(10000);
      long Result = 0;
      switch (System.IntPtr.Size)
      {
        case 4:
          Result = CoolPropDLLfunc_x86(param, output, output.Capacity);
          break;
        case 8:
          Result = CoolPropDLLfunc_x64(param, output, output.Capacity);
          break;
        default:
          throw new System.Exception("Unknown platform!");
      }
      coolpropPlugin.LogInfo("[INFO ]", "param = {0} output = {1} Result = {2}", param, resultStr = output.ToString(), Result);
      return (Result == 1);
    }

    Term inf;
    public static int[] Arguments = new [] {1};

    public CoolProp_get_global_param_string(int childCount)
    {
      inf = new Term(this.GetType().Name, TermType.Function, childCount);
    }

    Term IFunction.Info { get { return inf; } }

    TermInfo IFunction.GetTermInfo(string lang)
    {
      string funcInfo = "(ParamName) Get a globally-defined string\r\n" +
        "ParamName A string, one of \"version\", \"errstring\", \"warnstring\", \"gitrevision\", \"FluidsList\", \"fluids_list\", \"parameter_list\",\"predefined_mixtures\"";

      var argsInfos = new [] {
        new ArgumentInfo(ArgumentSections.String)
      };

      return new TermInfo(inf.Text,
                          inf.Type,
                          funcInfo,
                          FunctionSections.Unknown,
                          true,
                          argsInfos);
    }

    bool IFunction.ExpressionEvaluation(Term root, Term[][] args, ref SMath.Math.Store context, ref Term[] result)
    {
        var param = coolpropPlugin.GetStringParam(args[0], ref context);
        string resultStr;
        if (!CoolPropDLLfunc(param, out resultStr))
            coolpropPlugin.CoolPropError();
        result = coolpropPlugin.MakeStringResult(resultStr);
        return true;
    }
  }
}
