using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ArvoreGeradora
{
    public class GeradorPMML
    {
        private List<Atributo> TodosAtributos;
        private string AtributoMeta;
        private int IdNode = 0;
        private Arvore Arvore;
        private HashSet<string> PercentuaisClasseMeta;

        public XmlDocument Gerar(List<Atributo> todosAtributos, string atributoMeta, Arvore arvore, HashSet<string> percentuaisClasseMeta)
        {
            this.TodosAtributos = todosAtributos;
            this.AtributoMeta = atributoMeta;
            this.Arvore = arvore;
            this.PercentuaisClasseMeta = percentuaisClasseMeta;

            //Gerar as configurações do xml
            XmlDocument doc = new XmlDocument();
            XmlDeclaration xmlDeclaration = doc.CreateXmlDeclaration("1.0", "UTF-8", null);

            XmlElement root = doc.DocumentElement;
            doc.InsertBefore(xmlDeclaration, root);

            var atrVersion = doc.CreateAttribute("version");
            atrVersion.Value = "4.1";
            var atrXmlns = doc.CreateAttribute("xmlns");
            atrXmlns.Value = "http://www.dmg.org/PMML-4_1";

            XmlElement elementPMML = doc.CreateElement(string.Empty, "PMML", string.Empty);
            elementPMML.Attributes.Append(atrVersion);
            elementPMML.Attributes.Append(atrXmlns);
            doc.AppendChild(elementPMML);
            GerarHeader(doc, elementPMML);
            GerarDataDictionary(doc, elementPMML);
            GerarTreeModelo(doc, elementPMML);

            return doc;
        }

        public void GerarHeader(XmlDocument doc, XmlElement elementoPMML)
        {
            var atrCopyright = doc.CreateAttribute("copyright");
            atrCopyright.Value = "Rafael.Semann";

            XmlElement elementHeader = doc.CreateElement(string.Empty, "Header", string.Empty);
            elementHeader.Attributes.Append(atrCopyright);

            var atrName = doc.CreateAttribute("name");
            atrName.Value = "Rafael.Semann";

            var atrVersion = doc.CreateAttribute("version");
            atrName.Value = "1.0.0";

            XmlElement eleApplication = doc.CreateElement(string.Empty, "Application", string.Empty);
            eleApplication.Attributes.Append(atrName);
            eleApplication.Attributes.Append(atrVersion);

            elementHeader.AppendChild(eleApplication);

            elementoPMML.AppendChild(elementHeader);
        }

        public void GerarDataDictionary(XmlDocument doc, XmlElement elementoPMML)
        {
            var atrNumberOfFields = doc.CreateAttribute("numberOfFields");
            atrNumberOfFields.Value = TodosAtributos.Count.ToString();

            XmlElement elementDataDictionary = doc.CreateElement(string.Empty, "DataDictionary", string.Empty);
            elementDataDictionary.Attributes.Append(atrNumberOfFields);

            foreach (var atributo in TodosAtributos)
            {
                XmlElement eleDataField = doc.CreateElement(string.Empty, "DataField", string.Empty);

                var atrName = doc.CreateAttribute("name");
                atrName.Value = atributo.Nome;

                var atrOptype = doc.CreateAttribute("optype");
                var atrDataType = doc.CreateAttribute("dataType");

                if (atributo.TipoAtributo == Tipo.Categorico || atributo.Nome == AtributoMeta)
                {
                    atrOptype.Value = "categorical";
                    atrDataType.Value = "string";
                }
                else
                {
                    atrOptype.Value = "continuous";
                    atrDataType.Value = "double";
                }

                if (atributo.Nome == AtributoMeta)
                {
                    foreach (var valor in PercentuaisClasseMeta)
                    {
                        var atrValue = doc.CreateAttribute("value");
                        atrValue.Value = valor.ToString();
                        XmlElement eleValue = doc.CreateElement(string.Empty, "Value", string.Empty);
                        eleValue.Attributes.Append(atrValue);

                        eleDataField.AppendChild(eleValue);
                    }
                }
                else if (atributo.TipoAtributo == Tipo.Categorico)
                {
                    foreach (var valor in atributo.RetornaValoresDistintos())
                    {
                        var atrValue = doc.CreateAttribute("value");
                        atrValue.Value = valor.ToString();
                        XmlElement eleValue = doc.CreateElement(string.Empty, "Value", string.Empty);
                        eleValue.Attributes.Append(atrValue);

                        eleDataField.AppendChild(eleValue);
                    }
                }
                else
                {
                    var atrClosure = doc.CreateAttribute("closure");
                    atrClosure.Value = "closedClosed";
                    var atrLeftMargin = doc.CreateAttribute("leftMargin");
                    atrLeftMargin.Value = "0";
                    var atrRightMargin = doc.CreateAttribute("rightMargin");
                    atrRightMargin.Value = "100000000000";
                    XmlElement eleInterval = doc.CreateElement(string.Empty, "Interval", string.Empty);
                    eleInterval.Attributes.Append(atrClosure);
                    eleInterval.Attributes.Append(atrLeftMargin);
                    eleInterval.Attributes.Append(atrRightMargin);

                    eleDataField.AppendChild(eleInterval);
                }

                eleDataField.Attributes.Append(atrName);
                eleDataField.Attributes.Append(atrOptype);
                eleDataField.Attributes.Append(atrDataType);

                elementDataDictionary.AppendChild(eleDataField);
            }

            elementoPMML.AppendChild(elementDataDictionary);
        }

        public void GerarTreeModelo(XmlDocument doc, XmlElement elementoPMML)
        {
            XmlElement elementTreeModel = doc.CreateElement(string.Empty, "TreeModel", string.Empty);


            var atrModelName = doc.CreateAttribute("modelName");
            atrModelName.Value = "DecisionTree";

            var atrFunctionName = doc.CreateAttribute("functionName");
            atrFunctionName.Value = "classification";

            var atrSplitCharacteristic = doc.CreateAttribute("splitCharacteristic");
            atrSplitCharacteristic.Value = "multiSplit";

            var atrMissingValueStrategy = doc.CreateAttribute("missingValueStrategy");
            atrMissingValueStrategy.Value = "lastPrediction";

            var atrNoTrueChildStrategy = doc.CreateAttribute("noTrueChildStrategy");
            atrNoTrueChildStrategy.Value = "returnNullPrediction";

            elementTreeModel.Attributes.Append(atrModelName);
            elementTreeModel.Attributes.Append(atrFunctionName);
            elementTreeModel.Attributes.Append(atrSplitCharacteristic);
            elementTreeModel.Attributes.Append(atrMissingValueStrategy);
            elementTreeModel.Attributes.Append(atrNoTrueChildStrategy);

            //Gerar o mining Schema
            XmlElement elementMiningSchema = doc.CreateElement(string.Empty, "MiningSchema", string.Empty);


            //Gerar os mining field
            foreach (var atributo in TodosAtributos)
            {
                XmlElement eleMiningField = doc.CreateElement(string.Empty, "MiningField", string.Empty);

                var atrName = doc.CreateAttribute("name");
                atrName.Value = atributo.Nome;

                var atrInvalidValueTreatment = doc.CreateAttribute("invalidValueTreatment");
                atrInvalidValueTreatment.Value = "asIs";

                eleMiningField.Attributes.Append(atrName);
                eleMiningField.Attributes.Append(atrInvalidValueTreatment);

                if (atributo.Nome == AtributoMeta)
                {
                    var atrUsageType = doc.CreateAttribute("usageType");
                    atrUsageType.Value = "predicted";
                    eleMiningField.Attributes.Append(atrUsageType);
                }

                elementMiningSchema.AppendChild(eleMiningField);
            }

            elementTreeModel.AppendChild(elementMiningSchema);
            elementoPMML.AppendChild(elementTreeModel);

            GerarNode(doc, elementTreeModel);
        }

        public void GerarNode(XmlDocument doc, XmlElement elementoTreeModel)
        {
            XmlElement elementNode = doc.CreateElement(string.Empty, "Node", string.Empty);
            elementoTreeModel.AppendChild(elementNode);

            var atrId = doc.CreateAttribute("id");
            atrId.Value = IdNode.ToString();

            var atrScore = doc.CreateAttribute("score");
            atrScore.Value = Arvore.NoRaiz.Percentual.ToString() + "%";
            elementNode.Attributes.Append(atrId);
            elementNode.Attributes.Append(atrScore);

            //No raiz
            XmlElement elementTrue = doc.CreateElement(string.Empty, "True", string.Empty);
            elementNode.AppendChild(elementTrue);

            foreach (var no in Arvore.NoRaiz.NosFilhos)
            {
                IdNode++;
                GerarNodeFilhos(doc, elementNode, no);
            }
        }

        public void GerarNodeFilhos(XmlDocument doc, XmlElement elementoAcima, No no)
        {
            XmlElement elementNode = doc.CreateElement(string.Empty, "Node", string.Empty);
            elementoAcima.AppendChild(elementNode);

            var atrId = doc.CreateAttribute("id");
            atrId.Value = IdNode.ToString();

            var atrScore = doc.CreateAttribute("score");
            atrScore.Value = no.MaiorValorClasseMeta;

            elementNode.Attributes.Append(atrId);
            elementNode.Attributes.Append(atrScore);

            //Verificação
            XmlElement elementSimplePredicate = doc.CreateElement(string.Empty, "SimplePredicate", string.Empty);

            var atrField = doc.CreateAttribute("field");
            atrField.Value = no.Nome;

            var atrOperator = doc.CreateAttribute("operator");
            if (no.Tipo != Tipo.Contínuo)
                atrOperator.Value = "equal";
            else
            {
                if (no.MenorIgual)
                    atrOperator.Value = "lessOrEqual";
                else
                    atrOperator.Value = "greaterThan";
            }

            var atrValue = doc.CreateAttribute("value");
            atrValue.Value = no.Valor;

            elementSimplePredicate.Attributes.Append(atrField);
            elementSimplePredicate.Attributes.Append(atrOperator);
            elementSimplePredicate.Attributes.Append(atrValue);

            elementNode.AppendChild(elementSimplePredicate);

            foreach (var noFilho in no.NosFilhos)
            {
                IdNode++;
                GerarNodeFilhos(doc, elementNode, noFilho);
            }
        }
    }
}
