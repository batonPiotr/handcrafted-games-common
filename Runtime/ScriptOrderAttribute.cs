namespace HandcraftedGames.Common
{
    using System;
    public class ScriptOrder: Attribute
    {
        public int order;

        public ScriptOrder(int order)
        {
            this.order = order;
        }
    }
}