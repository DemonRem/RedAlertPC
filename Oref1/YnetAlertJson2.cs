using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Oref1
{
    // jsonCallback({"alerts":{"items":[{"item":{"guid":"73510","pubdate":"22:27","title":"באר שבע 286","description":"בית קמה, גבעות בר, טארבין, משמר הנגב, רהט, שובל, להבים, דביר","link":"http://www.oref.org.il"}},{"item":{"guid":"73511","pubdate":"22:28","title":"באר שבע 289","description":"אשל הנשיא , בטחה, גילת, תפרח","link":"http://www.oref.org.il"}}]}});

    public class YnetAlertJson2
    {
        public YnetAlertSubJson2 alerts { get; set; }
    }
}
