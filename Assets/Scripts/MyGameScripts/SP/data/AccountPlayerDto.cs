public class AccountPlayerDto
{
	//用户角色id
	public long id;
	//角色昵称
	public string nickname;
	//角色等级
	public int grade;
	//主色角色id
	public int charactorId;
	//角色头像图标
	public int icon; //这个后面可以废弃，现在保留是兼容旧数据
	//角色门派
	public int factionId;
	//角色所在服务器
	public int gameServerId;
	//角色最近登录时间
	public long recentLoginTime;
}