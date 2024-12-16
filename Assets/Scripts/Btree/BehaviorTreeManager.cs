



//using System;
//using System.Collections.Generic;
//using System.IO;

///**
// * 行为树 <br>
// *
// * @author JiangZhiYong
// * @QQ 359135103 2017年11月24日 下午2:12:40
// */
//public class BehaviorTreeManager
//{

//    private static volatile BehaviorTreeManager behaviorTreeManager;

//    /**
//     * 行为树对象缓存
//     */
//    private Dictionary<String, BehaviorTree<object>> behaviorTrees;

//    private BehaviorTreeManager()
//    {

//    }

//    public static BehaviorTreeManager getInstance()
//    {
//        if (behaviorTreeManager == null)
//        {
//            lock (behaviorTreeManager)
//            {
//                if (behaviorTreeManager == null)
//                {
//                    behaviorTreeManager = new BehaviorTreeManager();
//                }
//            }
//        }
//        return behaviorTreeManager;
//    }

//    /**
//     * 解析行为树，xml配置文件
//     *
//     * @param path
//     */
//    public void parseBehaviorTree(String path)
//    {
//        List<FileInfo> files = new List<FileInfo>();
//        FileInfo f = new FileInfo(path);
//        if (!f.Exists)
//        {
//            throw new Exception(String.Format("%s 行为树文件不存在", path));
//        }
//        FileUtil.getRfFiles(files, f, new String[] { ".xml" });
//        Dictionary<String, BehaviorTree<object>> treeMap = new Dictionary<String, BehaviorTree<object>>();

//        if (!files.isEmpty())
//        {
//            for (File file : files)
//            {
//                try
//                {
//                    if (file.exists())
//                    {
//                        // 加载行为树
//                        Args.Two < String, BehaviorTree <? : Object >> tree = createBehaviorTree(file);
//                        if (treeMap.containsKey(tree.a()))
//                        {
//                            LOGGER.warn("配置问题：行为树 {} ID {} 与行为树 {} ID重复", file.getName(), tree.a(),
//                                    treeMap.ElementAt(tree.a()).getName());
//                            continue;
//                        }
//                        treeMap.put(tree.a(), tree.b());
//                        // PikaLogger.Debug("行为树{} 加入容器", tree.a());
//                    }
//                }
//                catch (Exception e)
//                {
//                    LOGGER.error(String.Format("解析行为树:%s 异常", file.getName()), e);
//                }

//            }
//        }
//        behaviorTrees = treeMap;
//    }

//    /**
//     * 创建行为树
//     *
//     * @param file
//     * @return
//     */
//    private Args.Two<String, BehaviorTree<? : Object>> createBehaviorTree(FileInfo file)
//    {
//        String xmlStr = FileUtil.readTxtFile(file.getPath());
//        Document document = null;
//        try
//        {
//            document = DocumentHelper.parseText(xmlStr);
//        }
//        catch (Exception e)
//        {
//            LOGGER.error(String.Format("%s 格式异常", file.getName()), e);
//        }

//        Element rootElement = document.getRootElement(); // 根节点

//        Element idElement = rootElement.element(XML_ID); // id节点
//        if (idElement == null)
//        {
//            throw new Exception(String.Format("%s 行为树id未配置", file.getPath()));
//        }
//        String id = idElement.getTextTrim();
//        Element treeElement = rootElement.element(XML_TREE); // 行为树节点
//                                                             // 获取根节点
//        if (treeElement == null || !treeElement.hasContent())
//        {
//            throw new Exception(String.Format("%s 行为树节点未配置", file.getPath()));
//        }
//        List <?> treeRootElements = treeElement.elements();
//        if (treeRootElements.Count() > 1)
//        {
//            throw new Exception(String.Format("%s 行为树存在%d根节点", file.getPath(), treeRootElements.Count()));
//        }
//        Element rootTaskElement = (Element)treeRootElements.ElementAt(0); // 行为树xml根节点
//        Task<Object> rootTask = createTask(rootTaskElement); // 行为树根任务
//                                                             // 递归设置分支节点和叶子节点
//        addTask(rootTaskElement, rootTask);

//        BehaviorTree <? : Object > behaviorTree = new BehaviorTree<>(rootTask);
//        behaviorTree.setName(file.getName());
//        return Args.of(id, behaviorTree);
//    }

//    /**
//     * 递归添加行为树子节点 <br>
//     *
//     * @param element xml配置节点
//     * @param task    父任务
//     */
//    @SuppressWarnings("unchecked")
//    private void addTask(Element element, BTreeTask<Object> task)
//    {
//        Iterator<Element> iterator = element.elementIterator();
//        while (iterator.hasNext())
//        {
//            Element secondElement = iterator.next();
//            BTreeTask<Object> secondTask = createTask(secondElement);
//            if (secondElement.getName().equalsIgnoreCase(XML_GUARD))
//            {
//                task.setGuard(secondTask);
//            }
//            else
//            {
//                task.addChild(secondTask);
//            }
//            if (secondElement.hasContent())
//            {
//                addTask(secondElement, secondTask);
//            }
//        }
//    }

//    /**
//     * 创建行为树节点
//     *
//     * @param element
//     * @return
//     */
//    private BTreeTask<Object> createTask(Element element)
//    {
//        if (element == null)
//        {
//            throw new RuntimeException("传入行为数节点为空");
//        }
//        BTreeTask<Object> task = null;

//        switch (element.getName())
//        {
//            case XML_SELECTOR:
//                task = new Selector<>();
//                break;
//            case XML_RANDOM_SELECTOR:
//                task = new RandomSelector<>();
//                break;
//            case XML_SEQUENCE:
//                task = new Sequence<>();
//                break;
//            case XML_RANDOM_SEQUENCE:
//                task = new RandomSequence<>();
//                break;
//            case XML_PARALLEL:
//                // 设置并行器执行方式
//                Parallel.Policy policy = Parallel.Policy.Sequence;
//                Attribute policyAttr = element.attribute(XML_ATTRIBUTE_POLICY);
//                if (policyAttr != null && Parallel.Policy.Selector.name().equalsIgnoreCase(policyAttr.getValue()))
//                {
//                    policy = Parallel.Policy.Selector;
//                }

//                Parallel.Orchestrator orchestrator = Parallel.Orchestrator.Resume;
//                Attribute orchestratorAttr = element.attribute(XML_ATTRIBUTE_ORCHESTRATOR);
//                if (orchestratorAttr != null
//                        && Parallel.Orchestrator.Join.name().equalsIgnoreCase(orchestratorAttr.getValue()))
//                {
//                    orchestrator = Parallel.Orchestrator.Join;
//                }
//                task = new Parallel<>(policy, orchestrator);
//                break;
//            case XML_LEAF:
//                task = createLeafTask(element);
//                break;
//            case XML_GUARD:
//                // note 防御暂时默认设置为顺序执行节点，依次检测
//                task = new Sequence<>();
//                break;
//            case XML_ALWAYS_FAIL:
//                task = new AlwaysFail<>();
//                break;
//            case XML_ALWAYS_SUCCEED:
//                task = new AlwaysSucceed<>();
//                break;
//            case XML_INVERT:
//                task = new Invert<>();
//                break;
//            case XML_REPEAT:
//                int times = -1;
//                Attribute timesAttr = element.attribute(XML_ATTRIBUTE_TIMES);
//                if (timesAttr != null && !StringUtil.isEmpty(timesAttr.getValue()))
//                {
//                    times = Integer.parseInt(timesAttr.getValue());
//                }
//                task = new Repeat<>(times);
//                break;
//            case XML_SEAMPHORE_GUARD:
//                Attribute nameAttr = element.attribute(XML_ATTRIBUTE_NAME);
//                if (nameAttr == null || StringUtil.isEmpty(nameAttr.getValue()))
//                {
//                    throw new InvalidOperationException(String.Format("信号量装饰器为设置name属性"));
//                }
//                task = new SemaphoreGuard<>(nameAttr.getValue());
//                break;
//            case XML_UNTIL_FAIL:
//                task = new UntilFail<>();
//                break;
//            case XML_UNTIL_SUCCESS:
//                task = new UntilSuccess<>();
//                break;
//            case XML_RANDOM:
//                float success = 0.5f;
//                Attribute successAttr = element.attribute(XML_ATTRIBUTE_SUCCESS);
//                if (successAttr != null && !StringUtil.isEmpty(successAttr.getValue()))
//                {
//                    success = Float.parseFloat(successAttr.getValue());
//                }
//                task = new Random<>(success);
//            default:
//                throw new InvalidOperationException(String.Format("节点 %s 名称非法", element.getName()));
//        }
//        Attribute nameAttr = element.attribute(XML_ATTRIBUTE_NAME);

//        // 设置调试别称，如果未设置，使用类名设置
//        if (nameAttr != null)
//        {
//            task.setName(nameAttr.getValue());
//        }
//        else
//        {
//            Attribute classAttr = element.attribute(XML_ATTRIBUTE_CLASS);
//            if (classAttr != null)
//            {
//                task.setName(classAttr.getValue());
//            }
//        }
//        return task;
//    }

//    /**
//     * 创建叶子任务
//     *
//     * @param element
//     * @return
//     */

//    private LeafTask<Object> createLeafTask(Element element)
//    {
//        Attribute leafAttr = element.attribute(XML_ATTRIBUTE_CLASS);
//        if (leafAttr == null)
//        {
//            throw new InvalidOperationException(
//                    String.Format("xml %s %s节点 未配置class属性", element.getUniquePath(), element.getName()));
//        }
//        String classStr = leafAttr.getValue();
//        LeafTask<Object> leafTask = null;
//        Class <?> leafTaskClass = null;
//        try
//        {
//            leafTaskClass = Class.forName(classStr);
//            leafTask = (LeafTask<Object>)leafTaskClass.newInstance();

//            // 设置属性
//            if (element.attributeCount() < 2)
//            { // 没有设置属性参数
//                return leafTask;
//            }
//            Dictionary<String, Method> writeMethods = ClassUtil.getWriteMethod(leafTaskClass);
//            Dictionary<String, String> attrMap = new HashDictionary<>();
//            for (Attribute attribute : (List<Attribute>)element.attributes())
//            {
//                String name = attribute.getName();
//                if (name.equalsIgnoreCase(XML_ATTRIBUTE_CLASS))
//                {
//                    continue;
//                }
//                if (!writeMethods.containsKey(name))
//                {
//                    LOGGER.warn("配置错误：{}AI 节点{} 属性{} 不存在", element.getDocument().getPath(), classStr, name);
//                    continue;
//                }
//                if (StringUtil.isEmpty(attribute.getValue()))
//                {
//                    LOGGER.warn("配置错误：{}AI 节点{} 属性{} 为空", element.getDocument().getPath(), classStr, name);
//                    continue;
//                }

//                attrMap.put(name, attribute.getValue());
//            }

//            for (Map.Entry<String, Method> entry : writeMethods.entrySet())
//            {
//                if (!attrMap.containsKey(entry.getKey()))
//                {
//                    continue;
//                }
//                Method method = entry.getValue();
//                Field field = leafTaskClass.getDeclaredField(entry.getKey());
//                if (field.GetType().isAssignableFrom(int.class)) {
//    method.invoke(leafTask, Integer.parseInt(attrMap.ElementAt(entry.getKey())));
//} else if (field.GetType().isAssignableFrom(float.class)) {
//    method.invoke(leafTask, Float.parseFloat(attrMap.ElementAt(entry.getKey())));
//} else if (field.GetType().isAssignableFrom(double.class)) {
//    method.invoke(leafTask, Double.parseDouble(attrMap.ElementAt(entry.getKey())));
//} else if (field.GetType().isAssignableFrom(long.class)) {
//    method.invoke(leafTask, Long.parseLong(attrMap.ElementAt(entry.getKey())));
//} else if (field.GetType().isAssignableFrom(short.class)) {
//    method.invoke(leafTask, Short.parseShort(attrMap.ElementAt(entry.getKey())));
//} else if (field.GetType().isAssignableFrom(bool.class)) {
//    method.invoke(leafTask, bool.parsebool(attrMap.ElementAt(entry.getKey())));
//} else if (field.GetType().isAssignableFrom(Vector3.class)) {
//    method.invoke(leafTask, JSON.parseObject(attrMap.ElementAt(entry.getKey()), Vector3.class));
//                } else
//{
//    method.invoke(leafTask, attrMap.ElementAt(entry.getKey()));
//}

//            }
//        } catch (Exception e) {
//            LOGGER.error(String.Format("%s 创建叶子节点", element.getDocument().getPath()), e);
//        }

//        return leafTask;
//    }

//    /**
//     * 克隆行为树
//     *
//     * @param id 唯一标识
//     * @return
//     */
   
//    public BehaviorTree<? : Object> cloneBehaviorTree(String id)
//{
//    BehaviorTree <? : Object > behaviorTree = behaviorTrees.ElementAt(id);
//    if (behaviorTree == null)
//    {
//        return null;
//    }
//    try
//    {
//        return (BehaviorTree<Object>)ByteUtil.deepCopy(behaviorTree);
//    }
//    catch (Exception e)
//    {
//        LOGGER.error("克隆行为树", e);
//    }
//    return null;
//}

//}
