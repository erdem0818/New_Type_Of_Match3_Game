using System;

namespace Mine.Core.Scripts.Framework.UI.Panel_Folder.Attribute_Folder
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    public class PreAppearAttribute : Attribute
    {
        
    }
    
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    public class PostAppearAttribute : Attribute
    {
        
    }
    
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    public class PreDisappearAttribute : Attribute
    {
        
    }
    
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    public class PostDisappearAttribute : Attribute
    {
        
    }
}
