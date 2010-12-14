﻿KISS core

核心的一些接口

版本历史：

v1.6
增加获取QueryCondition和Repository获取DataTable的接口，处理自定义的查询
重要更新：移除QueryCondition的AppendWhereKeyword属性，由用户自行设置是否有where
重要更新：移除QueryObject的Query属性，替换为CreateContext方法
修复了Obj的一个bug

v1.5
新增IExtendable接口
移除了Log类和LogInitializer类，由日志提供者初始化自身
忽略ReflectionTypeLoadException异常
移除TemplateEngineInitializer，改由提供者自身提供
增加IUser接口，用于描述用户的信息

v1.4
增加了图像处理的工具类
类Principal增加PermissionDenied事件
JCache增加RemoveHierarchyCache方法，用于移除二级缓存
简化QueryCondition，移除了缓存相关代码
移除了ICachable接口
IRepository接口增加了Delete方法
查询允许使用“-”,“,”字符
添加了TemplateEngineInitializer，用于加载模板引擎插件
增加了ResourceUtil类，用于加载资源
增加DbProviderAttribute类，用于标识database provider
ServiceLocator类添加public IList<Type> Find(Type requestedType, bool excludeSelf)方法
PluginBootstrapper支持初始化插件中的插件
移除QueryCondition类的OrderbyItems属性，添加AppendOrderby方法

v1.3
增加了Repository的providers配置
修改QueryCondition的PageSize默认值为-1，表示没有设置，0表示不分页
文档注释
IRepository<T>接口增加了IKissQueryable<T> CreateQuery()方法
修复ServerUtil类的ResolveUrl方法的bug
ServiceLocator非初始化时调用抛出异常信息

v1.2
记录插件加载日志
修改ILinqQuery<T>接口为ILinqQueryable<T>
不在支持.net framework 2.0，支持的最低版本是3.5

v1.1
迁移到vs2010
IRepository<T>接口增加GetsAll()方法，QueryObject增加了相应的便利方法
实体类继承于QueryObject<T>或QueryObject<T>，由于c#不支持多重继承，所以业务实体类无法使用继承的特性。这个问题影响不是很大，业务模型采用组合的方式代替继承可以解决这个问题。如果业务模型必须使用继承，可以调用QueryObject的静态方法实现同样的功能，只是写法上不太简洁直观。
QueryCondition类增加了AddOrderby方法，用于通过程序添加排序信息；同时删除了AddOrderbyColumns方法
修复了QueryCondition的count缓存key的一个bug
修改Obj的Id为virtual, 修改了linq相关标签的名称
支持.net framework 2.0, 但是必须引用System.Core.dll
RepositoryInitializer支持线程安全的Repository（待测试）
由于.net framework对泛型的反射在不同版本下有不一致的表现，实体类不再支持继承
增加对AppDomainTypeFinder的异常处理
QueryCondition增加了EnableFireEventMulti属性
QueryCondition默认TableField修改为*,修复PageCount的bug
缓存系统支持对某些模型设置是否启用缓存
去除了QueryCondition排序列的“[]”

v1.0
支持对模型类设置连接字符串
从Logging Component迁移了LogInitializer
ServerUtil的小改动（部分方法增加了HttpResponse参数）
