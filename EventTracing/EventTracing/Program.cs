using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventSourceTransferEventEx
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var listener = new MyEventListener())
            {
                var relatedActivityId = Guid.Parse("abb50c46-c0b5-4cfc-b1f0-f5d19d3501d1");
                var activityId = Guid.Parse("becfb032-78e6-44e1-91e9-025e4f2c9ba0");

                MyEventSource.SetCurrentThreadActivityId(activityId);

                listener.EnableEvents(MyEventSource.Log, EventLevel.LogAlways);

                // keeps activity Id in EventArgs; relatedId = Guid.Empty
                MyEventSource.Log.EventEx(1, "test");

                // passes both ActivityId and RelatedActivityId to EventListener callback
                MyEventSource.Log.TransferEventEx(relatedActivityId, 1, "test");

                var aList = new List<int> { 3, 4, 5, 6 };
                var aDictionary = new Dictionary<string, int>() { { "user1", 1 }, { "user2", 2 } };
                var aNode = new NodeData
                {
                    NodeName = "Test",
                    NodeID = 1,
                    ChildNodeIds = new int[] { 3, 4, 5 },
                    UserValues = aDictionary
                };
                MyEventSource.Log.LogArray("testMessage", aList);

            }
        }
    }
    /*
    public sealed class MyEventSource : EventSource
    {
        public static MyEventSource Log = new MyEventSource();

        [Event(1, Message="ReqId: {0}, Title:{1}")]
        public void TransferEventEx(Guid relatedActivityId, int reqId, string title)
        {
            WriteEventWithRelatedActivityId(1, relatedActivityId, reqId, title);
        }

        [Event(2, Message = "Transfer Event")]
        public void EventEx(int reqId, string title)
        {
            WriteEvent(2, reqId, title);
        }
    }

    public class MyEventListener : EventListener
    {
        protected override void OnEventWritten(EventWrittenEventArgs eventData)
        {
            //throw new NotImplementedException();
            var activityId = eventData.ActivityId;
            var relatedActivityId = eventData.RelatedActivityId;

            Console.WriteLine("ActivityId: {0}, RelatedActivityId :{1}", activityId, relatedActivityId);
        }
    }
     * */
}
