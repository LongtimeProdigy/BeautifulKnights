using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Helpers
{
    public class ColorsDefinition
    {
        private static readonly Color _errorCode = new Color(1, 0.5f, 0.5f);
        public static Color errorCode { get { return _errorCode; } }

        private static readonly Color _defaultBackground = new Color(0.9f, 0.9f, 0.9f);
        public static Color defaultBackground { get { return _defaultBackground; } } 
    }
}
