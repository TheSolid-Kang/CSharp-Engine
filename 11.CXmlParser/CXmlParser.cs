using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Engine._10.CXmlParser
{
    internal class CXmlParser
    {
        private string _xmlFilePath;

        public CXmlParser(string xmlFilePath)
        {
            _xmlFilePath = xmlFilePath;
        }

        public List<XmlNode> ParseXml()
        {
            List<XmlNode> nodes = new List<XmlNode>();

            XmlDocument xmlDoc = new XmlDocument();
            try
            {
                xmlDoc.Load(_xmlFilePath);
                XmlNodeList nodeList = xmlDoc.DocumentElement.SelectNodes("/Root/Item");

                foreach (XmlNode node in nodeList)
                {
                    nodes.Add(node);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("XML 파싱 중 오류 발생: " + ex.Message);
            }

            return nodes;
        }

        public void PrintNodes(List<XmlNode> nodes)
        {
            foreach (var node in nodes)
            {
                Console.WriteLine("노드 이름: " + node.Name);
                Console.WriteLine("내용: " + node.InnerText);
                Console.WriteLine("-------------------");
            }
        }
    }
    public class XmlFileGenerator
    {
        public void CreateXmlFile(string filePath)
        {
            XmlDocument xmlDoc = new XmlDocument();

            // 루트 요소 생성
            XmlElement root = xmlDoc.CreateElement("Root");
            xmlDoc.AppendChild(root);

            // 자식 요소 생성
            for (int i = 1; i <= 5; i++)
            {
                XmlElement item = xmlDoc.CreateElement("Item");
                item.SetAttribute("id", i.ToString());

                XmlElement name = xmlDoc.CreateElement("Name");
                name.InnerText = "Item " + i;

                XmlElement value = xmlDoc.CreateElement("Value");
                value.InnerText = "Value " + i;

                item.AppendChild(name);
                item.AppendChild(value);
                root.AppendChild(item);
            }

            // XML 파일 저장
            try
            {
                xmlDoc.Save(filePath);
                Console.WriteLine("XML 파일이 성공적으로 생성되었습니다: " + filePath);
            }
            catch (Exception ex)
            {
                Console.WriteLine("파일 저장 중 오류 발생: " + ex.Message);
            }
        }
    }

    public class ItemDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
    }

    public class RootDto
    {
        public List<ItemDto> Items { get; set; } = new List<ItemDto>();
    }
    public class XmlToDtoConverter
    {
        public RootDto ConvertXmlToDto(string xmlFilePath)
        {
            RootDto rootDto = new RootDto();

            XmlDocument xmlDoc = new XmlDocument();
            try
            {
                xmlDoc.Load(xmlFilePath);
                XmlNodeList nodeList = xmlDoc.DocumentElement.SelectNodes("/Root/Item");

                foreach (XmlNode node in nodeList)
                {
                    ItemDto itemDto = new ItemDto
                    {
                        Id = int.Parse(node.Attributes["id"].Value),
                        Name = node["Name"].InnerText,
                        Value = node["Value"].InnerText
                    };

                    rootDto.Items.Add(itemDto);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("XML 변환 중 오류 발생: " + ex.Message);
            }

            return rootDto;
        }
    }

    /*
class Program
{
    static void Main(string[] args)
    {
        string xmlFilePath = "path/to/your/xmlfile.xml"; // XML 파일 경로를 입력하세요.
        XmlToDtoConverter converter = new XmlToDtoConverter();
        
        RootDto result = converter.ConvertXmlToDto(xmlFilePath);

        foreach (var item in result.Items)
        {
            Console.WriteLine($"ID: {item.Id}, Name: {item.Name}, Value: {item.Value}");
        }
    }
}

    --
설명
DTO 클래스: ItemDto와 RootDto 클래스를 정의하여 XML 요소를 표현합니다.
XmlToDtoConverter 클래스: XML 파일을 로드하고, Item 노드를 찾아 DTO 객체로 변환하는 기능을 제공합니다.
사용 예제: XML 파일 경로를 설정하고 ConvertXmlToDto 메서드를 호출하여 DTO 객체로 변환한 후, 결과를 출력합니다.
이 코드를 사용하여 XML 데이터를 쉽게 DTO 객체로 변환할 수 있습니다. 필요에 따라 DTO 구조를 조정하거나 변환 로직을 확장할 수 있습니다.
     */
}

/*
class Program
{
    static void Main(string[] args)
    {
        string xmlFilePath = "path/to/your/xmlfile.xml"; // XML 파일 경로를 입력하세요.
        XmlParser parser = new XmlParser(xmlFilePath);
        
        List<XmlNode> nodes = parser.ParseXml();
        parser.PrintNodes(nodes);
    }
}


--------

<Root>
    <Item>
        <Name>Item 1</Name>
        <Value>Value 1</Value>
    </Item>
    <Item>
        <Name>Item 2</Name>
        <Value>Value 2</Value>
    </Item>
</Root>
 */