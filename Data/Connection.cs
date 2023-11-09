using System;

namespace ColorMixing.Data
{
    /// <summary>
    /// Data entity represents the connection between two objects
    /// </summary>
    public class Connection
    {
        /// <summary>
        /// Parent entity identifier
        /// </summary>
        public Guid ParentEntityId { get; set; }
        
        /// <summary>
        /// Child entity identifier
        /// </summary>
        public Guid ChildEntityId { get; set; }
        
        /// <summary>
        /// Connection line identifier
        /// </summary>
        public Guid LineId { get; set; }
    }
}