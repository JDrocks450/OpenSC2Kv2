namespace OpenSC2Kv2.API.IFF
{
    /// <summary>
    /// Provides framework around designing <see cref="SC2Segment"/> data processors.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class SegmentHandler
    {
        private static Dictionary<SC2SegmentTypes, SegmentHandler> RegisteredHandlers = new();
        public abstract IEnumerable<ISegmentItem> Process(SC2Segment Segment);
        public static void RegisterHandler(SC2SegmentTypes Type, SegmentHandler Handler)
        {
            RegisteredHandlers.Add(Type, Handler);
        }
        public static IEnumerable<ISegmentItem>? TryProcess(SC2Segment Segment)
        {
            if (!RegisteredHandlers.TryGetValue(Segment.SC2Type, out var handler))
                return null;
            return handler.Process(Segment);
        }
        static SegmentHandler()
        {
            //REGISTER HANDLERS
            RegisterHandler(SC2SegmentTypes.ALTM, new ALTMSegmentHandler());
            RegisterHandler(SC2SegmentTypes.XTER, new XTERSegmentHandler());
            RegisterHandler(SC2SegmentTypes.XBLD, new XBLDSegmentHandler());
            RegisterHandler(SC2SegmentTypes.XZON, new XZONSegmentHandler());
        }
    }
}
