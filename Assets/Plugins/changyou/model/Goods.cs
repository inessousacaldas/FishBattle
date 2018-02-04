namespace CySdk{
	
	public class Goods {

		private string goodsName;
		private int goodsNumber;
		private string goodsId;
		private string goodsRegisterId;
		private double goodsPrice;
		private string goodsIcon = "";
		private string goodsDescribe = "";
		private string pushInfo = "";
		private int type;

		public Goods(string goodsName, int goodsNumber, string goodsId, string goodsRegisterId, double goodsPrice)
		{
			this.goodsName = goodsName;
			this.goodsNumber = goodsNumber;
			this.goodsId = goodsId;
			this.goodsRegisterId = goodsRegisterId;
			this.goodsPrice = goodsPrice;
		}

		public string getGoodsName()
		{
			return this.goodsName;
		}

		public int getGoodsNumber()
		{
			return this.goodsNumber;
		}

		public string getGoodsId()
		{
			return this.goodsId;
		}

		public string getGoodsRegisterId()
		{
			return this.goodsRegisterId;
		}

		public double getGoodsPrice()
		{
			return this.goodsPrice;
		}

		public string getGoodsIcon()
		{
			return this.goodsIcon;
		}

		public Goods setGoodsIcon(string goodsIcon)
		{
			this.goodsIcon = goodsIcon;
			return this;
		}

		public string getGoodsDescribe()
		{
			if (this.goodsDescribe == "") {
				return this.goodsName;
			}
			return this.goodsDescribe;
		}

		public Goods setGoodsDescribe(string goodsDescribe)
		{
			this.goodsDescribe = goodsDescribe;
			return this;
		}

		public string getPushInfo()
		{
			return this.pushInfo;
		}

		public Goods setPushInfo(string pushInfo)
		{
			this.pushInfo = pushInfo;
			return this;
		}

		public int getType()
		{
			return this.type;
		}

		public Goods setType(int type)
		{
			this.type = type;
			return this;
		}
	}

}