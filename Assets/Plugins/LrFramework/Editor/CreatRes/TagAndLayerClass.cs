using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text;
using UnityEditorInternal;


public class AutoTagAndLayer
{
    public StringBuilder builder;
    public int order = 1;
    public AutoTagAndLayer()
    {
        builder = new StringBuilder();
        builder.AppendLine($"  {CreatUtil.GetFormat(order)}//自动生成类，不能修改");
        builder.AppendLine($"  {CreatUtil.GetFormat(order)}public class TagConst");
        builder.AppendLine($"  {CreatUtil.GetFormat(order)}{CreatUtil.LeftSymbol}");
        foreach (var item in InternalEditorUtility.tags)
        {
            builder.AppendLine($"   {CreatUtil.GetFormat(order)}{CreatUtil.SetStringField(item, item, order)}");
        }
        builder.AppendLine($"  {CreatUtil.GetFormat(order)}{CreatUtil.RightSymbol}");

        builder.AppendLine($"  {CreatUtil.GetFormat(order)}//自动生成类，不能修改");
        builder.AppendLine($"  {CreatUtil.GetFormat(order)}public class LayerConst");
        builder.AppendLine($"  {CreatUtil.GetFormat(order)}{CreatUtil.LeftSymbol}");
        foreach (var item in InternalEditorUtility.layers)
        {
            builder.AppendLine($"   {CreatUtil.GetFormat(order)}{CreatUtil.SetStringField(item,item,order)}");
        }
        builder.AppendLine($"  {CreatUtil.GetFormat(order)}{CreatUtil.RightSymbol}");
    }
}

