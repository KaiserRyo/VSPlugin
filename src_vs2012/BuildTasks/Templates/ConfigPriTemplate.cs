﻿// ------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version: 12.0.0.0
//  
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
// ------------------------------------------------------------------------------
namespace BlackBerry.BuildTasks.Templates
{
    using System.Text;
    using System.Collections.Generic;
    using System;
    
    /// <summary>
    /// Class to produce the template output
    /// </summary>
    
    #line 1 "T:\vs-plugin\src_vs2012\BuildTasks\Templates\ConfigPriTemplate.tt"
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "12.0.0.0")]
    internal partial class ConfigPriTemplate : ConfigPriTemplateBase
    {
#line hidden
        /// <summary>
        /// Create the template output
        /// </summary>
        public virtual string TransformText()
        {
            this.Write("#\r\n# Cascades Application config file for [");
            
            #line 5 "T:\vs-plugin\src_vs2012\BuildTasks\Templates\ConfigPriTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(SolutionName));
            
            #line default
            #line hidden
            this.Write("] created at [");
            
            #line 5 "T:\vs-plugin\src_vs2012\BuildTasks\Templates\ConfigPriTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(DateTime.Now));
            
            #line default
            #line hidden
            this.Write("] by BlackBerry NDK plugin for Visual Studio (v");
            
            #line 5 "T:\vs-plugin\src_vs2012\BuildTasks\Templates\ConfigPriTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Version));
            
            #line default
            #line hidden
            this.Write(").\r\n# Any manual changes made by user will be overwritten! Use the project settin" +
                    "gs instead.\r\n#\r\n\r\nBASEDIR = $$quote($$_PRO_FILE_PWD_)\r\n\r\n");
            
            #line 11 "T:\vs-plugin\src_vs2012\BuildTasks\Templates\ConfigPriTemplate.tt"
 if (!string.IsNullOrEmpty(PrecompiledHeaderName)) { 
            
            #line default
            #line hidden
            this.Write("#############################################\r\n# Precompiled header\r\nCONFIG += pr" +
                    "ecompile_header\r\nPRECOMPILED_HEADER = $$quote($$BASEDIR/");
            
            #line 15 "T:\vs-plugin\src_vs2012\BuildTasks\Templates\ConfigPriTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(PrecompiledHeaderName));
            
            #line default
            #line hidden
            this.Write(")\r\n");
            
            #line 16 "T:\vs-plugin\src_vs2012\BuildTasks\Templates\ConfigPriTemplate.tt"
 } 
            
            #line default
            #line hidden
            this.Write("\r\n#############################################\r\n# Libs, undefines and defines\r\n");
            
            #line 20 "T:\vs-plugin\src_vs2012\BuildTasks\Templates\ConfigPriTemplate.tt"
 if (HasAdditionalLibraryDirs) { 
            
            #line default
            #line hidden
            this.Write("QMAKE_LIBDIR += ");
            
            #line 21 "T:\vs-plugin\src_vs2012\BuildTasks\Templates\ConfigPriTemplate.tt"
 WriteRelativePaths(AdditionalLibraryDirs, "    \"", "\""); 
            
            #line default
            #line hidden
            
            #line 22 "T:\vs-plugin\src_vs2012\BuildTasks\Templates\ConfigPriTemplate.tt"
 } 
            
            #line default
            #line hidden
            
            #line 23 "T:\vs-plugin\src_vs2012\BuildTasks\Templates\ConfigPriTemplate.tt"
 if (HasDependencyLibrariesReferences()) { 
            
            #line default
            #line hidden
            this.Write("LIBS += ");
            
            #line 24 "T:\vs-plugin\src_vs2012\BuildTasks\Templates\ConfigPriTemplate.tt"
 WriteDependencyLibrariesReferences(); 
            
            #line default
            #line hidden
            
            #line 25 "T:\vs-plugin\src_vs2012\BuildTasks\Templates\ConfigPriTemplate.tt"
 } 
            
            #line default
            #line hidden
            this.Write("\r\n");
            
            #line 27 "T:\vs-plugin\src_vs2012\BuildTasks\Templates\ConfigPriTemplate.tt"

    WriteCollectionNewLine(UndefinePreprocessorDefinitions, "DEFINES -= ");
    WriteCollectionNewLine(PreprocessorDefinitions, "DEFINES += ");

            
            #line default
            #line hidden
            this.Write("\r\n#############################################\r\n# Source files, headers and QMLs" +
                    "\r\nSOURCES += ");
            
            #line 34 "T:\vs-plugin\src_vs2012\BuildTasks\Templates\ConfigPriTemplate.tt"
 WriteRelativePaths(CompileItems, "    $$quote($$BASEDIR/", ")"); 
            
            #line default
            #line hidden
            this.Write("\r\nHEADERS += ");
            
            #line 36 "T:\vs-plugin\src_vs2012\BuildTasks\Templates\ConfigPriTemplate.tt"
 WriteRelativePaths(IncludeItems, "    $$quote($$BASEDIR/", ")"); 
            
            #line default
            #line hidden
            this.Write("\r\nINCLUDEPATH += ");
            
            #line 38 "T:\vs-plugin\src_vs2012\BuildTasks\Templates\ConfigPriTemplate.tt"
 WriteRelativePaths(IncludeDirs, "    $$quote($$BASEDIR/", ")"); 
            
            #line default
            #line hidden
            this.Write("\r\n");
            
            #line 40 "T:\vs-plugin\src_vs2012\BuildTasks\Templates\ConfigPriTemplate.tt"
 if (HasAdditionalIncludeDirs) { 
            
            #line default
            #line hidden
            this.Write("INCLUDEPATH += ");
            
            #line 41 "T:\vs-plugin\src_vs2012\BuildTasks\Templates\ConfigPriTemplate.tt"
 WriteRelativePaths(AdditionalIncludeDirs, "    \"", "\""); 
            
            #line default
            #line hidden
            
            #line 42 "T:\vs-plugin\src_vs2012\BuildTasks\Templates\ConfigPriTemplate.tt"
 } 
            
            #line default
            #line hidden
            this.Write("\r\nOTHER_FILES += ");
            
            #line 44 "T:\vs-plugin\src_vs2012\BuildTasks\Templates\ConfigPriTemplate.tt"
 WriteRelativePaths(QmlItems, "    $$quote($$BASEDIR/", ")"); 
            
            #line default
            #line hidden
            this.Write("\r\n#############################################\r\n# Translations\r\nTRANSLATIONS = $" +
                    "$quote($${TARGET}.ts)\r\n\r\nlupdate_inclusion {\r\n\r\n    SOURCES += ");
            
            #line 52 "T:\vs-plugin\src_vs2012\BuildTasks\Templates\ConfigPriTemplate.tt"
 WriteRelativePathsTuple(CompileDirs, new[] { "*.c", "*.c++", "*.cc", "*.cpp", "*.cxx" }, "        $$quote($$BASEDIR/../", ")"); 
            
            #line default
            #line hidden
            this.Write("\r\n    SOURCES += ");
            
            #line 54 "T:\vs-plugin\src_vs2012\BuildTasks\Templates\ConfigPriTemplate.tt"
 WriteRelativePathsTuple(QmlDirs, new[] { "*.qml", "*.js", "*.qs" }, "        $$quote($$BASEDIR/../", ")"); 
            
            #line default
            #line hidden
            this.Write("\r\n    HEADERS += ");
            
            #line 56 "T:\vs-plugin\src_vs2012\BuildTasks\Templates\ConfigPriTemplate.tt"
 WriteRelativePathsTuple(IncludeDirs, new[] { "*.h", "*.h++", "*.hh", "*.hpp", "*.hxx" }, "        $$quote($$BASEDIR/../", ")"); 
            
            #line default
            #line hidden
            this.Write("\r\n}\r\n\r\n\r\n\r\n");
            return this.GenerationEnvironment.ToString();
        }
    }
    
    #line default
    #line hidden
    #region Base class
    /// <summary>
    /// Base class for this transformation
    /// </summary>
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "12.0.0.0")]
    internal class ConfigPriTemplateBase
    {
        #region Fields
        private global::System.Text.StringBuilder generationEnvironmentField;
        private global::System.CodeDom.Compiler.CompilerErrorCollection errorsField;
        private global::System.Collections.Generic.List<int> indentLengthsField;
        private string currentIndentField = "";
        private bool endsWithNewline;
        private global::System.Collections.Generic.IDictionary<string, object> sessionField;
        #endregion
        #region Properties
        /// <summary>
        /// The string builder that generation-time code is using to assemble generated output
        /// </summary>
        protected System.Text.StringBuilder GenerationEnvironment
        {
            get
            {
                if ((this.generationEnvironmentField == null))
                {
                    this.generationEnvironmentField = new global::System.Text.StringBuilder();
                }
                return this.generationEnvironmentField;
            }
            set
            {
                this.generationEnvironmentField = value;
            }
        }
        /// <summary>
        /// The error collection for the generation process
        /// </summary>
        public System.CodeDom.Compiler.CompilerErrorCollection Errors
        {
            get
            {
                if ((this.errorsField == null))
                {
                    this.errorsField = new global::System.CodeDom.Compiler.CompilerErrorCollection();
                }
                return this.errorsField;
            }
        }
        /// <summary>
        /// A list of the lengths of each indent that was added with PushIndent
        /// </summary>
        private System.Collections.Generic.List<int> indentLengths
        {
            get
            {
                if ((this.indentLengthsField == null))
                {
                    this.indentLengthsField = new global::System.Collections.Generic.List<int>();
                }
                return this.indentLengthsField;
            }
        }
        /// <summary>
        /// Gets the current indent we use when adding lines to the output
        /// </summary>
        public string CurrentIndent
        {
            get
            {
                return this.currentIndentField;
            }
        }
        /// <summary>
        /// Current transformation session
        /// </summary>
        public virtual global::System.Collections.Generic.IDictionary<string, object> Session
        {
            get
            {
                return this.sessionField;
            }
            set
            {
                this.sessionField = value;
            }
        }
        #endregion
        #region Transform-time helpers
        /// <summary>
        /// Write text directly into the generated output
        /// </summary>
        public void Write(string textToAppend)
        {
            if (string.IsNullOrEmpty(textToAppend))
            {
                return;
            }
            // If we're starting off, or if the previous text ended with a newline,
            // we have to append the current indent first.
            if (((this.GenerationEnvironment.Length == 0) 
                        || this.endsWithNewline))
            {
                this.GenerationEnvironment.Append(this.currentIndentField);
                this.endsWithNewline = false;
            }
            // Check if the current text ends with a newline
            if (textToAppend.EndsWith(global::System.Environment.NewLine, global::System.StringComparison.CurrentCulture))
            {
                this.endsWithNewline = true;
            }
            // This is an optimization. If the current indent is "", then we don't have to do any
            // of the more complex stuff further down.
            if ((this.currentIndentField.Length == 0))
            {
                this.GenerationEnvironment.Append(textToAppend);
                return;
            }
            // Everywhere there is a newline in the text, add an indent after it
            textToAppend = textToAppend.Replace(global::System.Environment.NewLine, (global::System.Environment.NewLine + this.currentIndentField));
            // If the text ends with a newline, then we should strip off the indent added at the very end
            // because the appropriate indent will be added when the next time Write() is called
            if (this.endsWithNewline)
            {
                this.GenerationEnvironment.Append(textToAppend, 0, (textToAppend.Length - this.currentIndentField.Length));
            }
            else
            {
                this.GenerationEnvironment.Append(textToAppend);
            }
        }
        /// <summary>
        /// Write text directly into the generated output
        /// </summary>
        public void WriteLine(string textToAppend)
        {
            this.Write(textToAppend);
            this.GenerationEnvironment.AppendLine();
            this.endsWithNewline = true;
        }
        /// <summary>
        /// Write formatted text directly into the generated output
        /// </summary>
        public void Write(string format, params object[] args)
        {
            this.Write(string.Format(global::System.Globalization.CultureInfo.CurrentCulture, format, args));
        }
        /// <summary>
        /// Write formatted text directly into the generated output
        /// </summary>
        public void WriteLine(string format, params object[] args)
        {
            this.WriteLine(string.Format(global::System.Globalization.CultureInfo.CurrentCulture, format, args));
        }
        /// <summary>
        /// Raise an error
        /// </summary>
        public void Error(string message)
        {
            System.CodeDom.Compiler.CompilerError error = new global::System.CodeDom.Compiler.CompilerError();
            error.ErrorText = message;
            this.Errors.Add(error);
        }
        /// <summary>
        /// Raise a warning
        /// </summary>
        public void Warning(string message)
        {
            System.CodeDom.Compiler.CompilerError error = new global::System.CodeDom.Compiler.CompilerError();
            error.ErrorText = message;
            error.IsWarning = true;
            this.Errors.Add(error);
        }
        /// <summary>
        /// Increase the indent
        /// </summary>
        public void PushIndent(string indent)
        {
            if ((indent == null))
            {
                throw new global::System.ArgumentNullException("indent");
            }
            this.currentIndentField = (this.currentIndentField + indent);
            this.indentLengths.Add(indent.Length);
        }
        /// <summary>
        /// Remove the last indent that was added with PushIndent
        /// </summary>
        public string PopIndent()
        {
            string returnValue = "";
            if ((this.indentLengths.Count > 0))
            {
                int indentLength = this.indentLengths[(this.indentLengths.Count - 1)];
                this.indentLengths.RemoveAt((this.indentLengths.Count - 1));
                if ((indentLength > 0))
                {
                    returnValue = this.currentIndentField.Substring((this.currentIndentField.Length - indentLength));
                    this.currentIndentField = this.currentIndentField.Remove((this.currentIndentField.Length - indentLength));
                }
            }
            return returnValue;
        }
        /// <summary>
        /// Remove any indentation
        /// </summary>
        public void ClearIndent()
        {
            this.indentLengths.Clear();
            this.currentIndentField = "";
        }
        #endregion
        #region ToString Helpers
        /// <summary>
        /// Utility class to produce culture-oriented representation of an object as a string.
        /// </summary>
        public class ToStringInstanceHelper
        {
            private System.IFormatProvider formatProviderField  = global::System.Globalization.CultureInfo.InvariantCulture;
            /// <summary>
            /// Gets or sets format provider to be used by ToStringWithCulture method.
            /// </summary>
            public System.IFormatProvider FormatProvider
            {
                get
                {
                    return this.formatProviderField ;
                }
                set
                {
                    if ((value != null))
                    {
                        this.formatProviderField  = value;
                    }
                }
            }
            /// <summary>
            /// This is called from the compile/run appdomain to convert objects within an expression block to a string
            /// </summary>
            public string ToStringWithCulture(object objectToConvert)
            {
                if ((objectToConvert == null))
                {
                    throw new global::System.ArgumentNullException("objectToConvert");
                }
                System.Type t = objectToConvert.GetType();
                System.Reflection.MethodInfo method = t.GetMethod("ToString", new System.Type[] {
                            typeof(System.IFormatProvider)});
                if ((method == null))
                {
                    return objectToConvert.ToString();
                }
                else
                {
                    return ((string)(method.Invoke(objectToConvert, new object[] {
                                this.formatProviderField })));
                }
            }
        }
        private ToStringInstanceHelper toStringHelperField = new ToStringInstanceHelper();
        /// <summary>
        /// Helper to produce culture-oriented representation of an object as a string
        /// </summary>
        public ToStringInstanceHelper ToStringHelper
        {
            get
            {
                return this.toStringHelperField;
            }
        }
        #endregion
    }
    #endregion
}
