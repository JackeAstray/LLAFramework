using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameLogic
{
    [AttributeUsage(AttributeTargets.Method)]
    public class RegisterCommandAttribute : Attribute
    {
        //最小参数数量
        int min_arg_count = 0;
        //最大参数数量
        int max_arg_count = -1;

        public int MinArgCount
        {
            get { return min_arg_count; }
            set { min_arg_count = value; }
        }

        public int MaxArgCount
        {
            get { return max_arg_count; }
            set { max_arg_count = value; }
        }

        //名称
        public string Name { get; set; }
        //帮助
        public string Help { get; set; }
        //提示
        public string Hint { get; set; }

        public RegisterCommandAttribute(string command_name = null)
        {
            Name = command_name;
        }
    }
}