using System;
using System.Runtime.InteropServices;

namespace IconPack
{
    /// <summary>
    /// Represents a resource name (either integer resource or string resource).
    /// </summary>
    public class ResourceName : IDisposable
    {
        #region Properties
        private int? _id;
        /// <summary>
        /// Gets the resource identifier, returns null if the resource is not an integer resource.
        /// </summary>
        public int? Id
        {
            get { return _id; }
            private set { _id = value; }
        }

        private string _name;
        /// <summary>
        /// Gets the resource name, returns null if the resource is not a string resource.
        /// </summary>
        public string Name
        {
            get { return _name; }
            private set { _name = value; }
        }

        private IntPtr _value;
        /// <summary>
        /// Gets a pointer to resource name that can be used in FindResource function.
        /// </summary>
        public IntPtr Value
        {
            get
            {
                if (IsIntResource)
                    if (Id != null) return new IntPtr(Id.Value);

                if (_value == IntPtr.Zero)
                    _value = Marshal.StringToHGlobalAuto(Name);

                return _value;
            }
            set { _value = value; }
        }

        /// <summary>
        /// Gets whether the resource is an integer resource.
        /// </summary>
        public bool IsIntResource
        {
            get { return (Id != null); }
        }
        #endregion

        #region Constructor/Destructor

        /// <summary>
        /// Initializes a new TAFactory.IconPack.ResourceName object.
        /// </summary>
        /// <param>Specifies the resource name. For more ifnormation, see the Remarks section.
        ///     <name>lpszName</name>
        /// </param>
        /// <param name="lpName"></param>
        /// <remarks>
        /// If the high bit of lpszName is not set (=0), lpszName specifies the integer identifier of the givin resource.
        /// Otherwise, it is a pointer to a null terminated string.
        /// If the first character of the string is a pound sign (#), the remaining characters represent a decimal number that specifies the integer identifier of the resource. For example, the string "#258" represents the identifier 258.
        /// #define IS_INTRESOURCE(_r) ((((ULONG_PTR)(_r)) >> 16) == 0).
        /// </remarks>
        public ResourceName(IntPtr lpName)
        {
            if (((uint)lpName >> 16) == 0)  //Integer resource
            {
                Id = lpName.ToInt32();
                Name = null;
            }
            else
            {
                Id = null;
                Name = Marshal.PtrToStringAuto(lpName);
            }
        }
        /// <summary>
        /// Destructs the ResourceName object.
        /// </summary>
        ~ResourceName()
        {
            Dispose();
        }
        #endregion

        #region Public Functions
        /// <summary>
        /// Returns a System.String that represents the current TAFactory.IconPack.ResourceName.
        /// </summary>
        /// <returns>Returns a System.String that represents the current TAFactory.IconPack.ResourceName.</returns>
        public override string ToString()
        {
            if (IsIntResource)
                return "#" + Id;

            return Name;
        }
        /// <summary>
        /// Releases the pointer to the resource name.
        /// </summary>
        public void Free()
        {
            if (_value != IntPtr.Zero)
            {
                try { Marshal.FreeHGlobal(_value); }
                catch
                {
                    // ignored
                }
                _value = IntPtr.Zero;
            }
        }
        #endregion

        #region IDisposable Members
        /// <summary>
        /// Release the pointer to the resource name.
        /// </summary>
        public void Dispose()
        {
            Free();
        }
        #endregion
    }
}
