using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Diagnostics.Tracing;
using System.Dynamic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace EventSourceTransferEventEx
{
    [EventData]
    public class SimpleData
    {
        public string Name { get; set; }
        public int Address { get; set; }
    }

    [EventData]
    public class NodeData
    {
        public string NodeName { get; set; }
        public int NodeId { get; set; }
        public int[] ChildNodeIds { get; set; }
        public Dictionary<string, int> UserValues { get; set; }
    }

    [EventSource(Name = "SampleEvents-Demo")]
    public sealed class Events : EventSource
    {
        public static Events Log = new Events();

        //[Event(1,Level=EventLevel.Informational,Message="Transfer,ReqID={1},Title={2}")]
        public void TransferEvent(Guid relatedId, int reqId, string title)
        {
            WriteEventWithRelatedActivityId(1, relatedId, reqId, title);
        }

        //[Event(2, Message = "Simple,ReqID={0},Title={1}")]
        public void SimpleEvent(int reqId, string title)
        {
            WriteEvent(2, reqId, title);
        }

        //[Event(3,Level=EventLevel.Informational,Message="")]
        public void LogNode(string message, NodeData data)
        {
            WriteEvent(3, message, data);
        }

        public void LogDictionary(string message, Dictionary<string, int> keyValues)
        {
            WriteEvent(4, message, keyValues);
        }

        public void LogSimpleData(string message, string data)
        {
            WriteEvent(5, message, data);
        }

        public void LogArray(string message, IEnumerable<int> data)
        {
            WriteEvent(6, message, data);
        }
    }

    public class MyEventListener : EventListener
    {
        private Dictionary<string, object> DictionaryFromKeyValues(object dictionary)
        {
            var ret = new Dictionary<string, object>();
            foreach (var item in dictionary as IEnumerable<object>)
            {
                var keyValue = item as IDictionary<string, object>;
                ret.Add((string)keyValue["Key"], keyValue["Value"]);
            }
            return ret;
        }

        protected override void OnEventWritten(EventWrittenEventArgs eventData)
        {
            switch (eventData.EventName)
            {
                case "LogArray":
                    {
                        string name = (string)eventData.Payload[0];
                        var arrValues = (IEnumerable<int>)eventData.Payload[1];
                        foreach (int arrValue in arrValues)
                        {
                            Console.WriteLine(arrValue);
                        }
                    } break;
                case "LogNode":
                    {
                        string name = (string)eventData.Payload[0];
                        var node = (IDictionary<string, object>)eventData.Payload[1];
                        var nodeName = node["Name"];
                        var nodeID = (int)node["NodeId"];
                        var childrenIDs = (IEnumerable<int>)node["ChildrenIds"];
                        var userValues = DictionaryFromKeyValues(node["UserValues"]);

                    } break;
                case "LogDictionary":
                    {
                        string name = (string)eventData.Payload[0];
                        var keyValues = DictionaryFromKeyValues(eventData.Payload[1]);
                        // Process the Dictionary

                    }
                    break;
            }
            var id = eventData.ActivityId;
            var rid = eventData.RelatedActivityId;
            var rName = eventData.EventName;
            Console.WriteLine("ActivityId: {0}, RelatedActivityId :{1}", id, rid);
        }
    }
}
