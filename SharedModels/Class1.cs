using NeoNix_QtPy_Models;

namespace IndicatorExample.Models
{
    internal class HdModel : IMessage
    {
        public string Title => typeof(HdModel).Name;
        public string HD { get; set; }
    }

}