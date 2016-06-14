var margin = { top: 0, right: 0, bottom: 20, left: 0 },
    width = 1100 - margin.right - margin.left,
    height = 3000 - margin.top - margin.bottom;

var i = 0,
    duration = 1500,
    root;

var tamanhoTotal = 0;

var tree = d3.layout.tree()
    .size([1000, width]);

var diagonal = d3.svg.diagonal()
 .projection(function (d) { return [d.x, d.y]; });

var svg = d3.select("#arvoreConsulta").append("svg")
    .attr("width", width + margin.right + margin.left)
    .attr("height", height + margin.top + margin.bottom)
    .style("padding-top", "30px")
    .style("margin-left", "-100px")
  .append("g")
    .attr("transform", "translate(" + margin.left + "," + margin.top + ")");

d3.json()
{
    $.ajax(
{
    url: "/ArvoreGerada/RetornaArvore",
    type: 'GET',
    cache: false,
    dataType: 'json',
    success: function (result) {
        root = result;
        root.x0 = 1000 / 2;
        root.y0 = 0;
        tamanhoTotal = root.size;
        update(root);
    },
    error: function (error) {
    }
});


};

d3.select(self.frameElement).style("height", "800px");

function update(source) {

    var nodes = tree.nodes(root).reverse(),
	  links = tree.links(nodes);

    // Normalize for fixed-depth.
    nodes.forEach(function (d) { d.y = d.depth * 180; });

    // Update the nodes…
    var node = svg.selectAll("g.node")
        .data(nodes, function (d) { return d.id || (d.id = ++i); });

    // Enter any new nodes at the parent's previous position.
    var nodeEnter = node.enter().append("g")
        .attr("class", "node")
        .attr("transform", function (d) { return "translate(" + source.x0 + "," + source.y0 + ")"; });

    nodeEnter.append("circle")
        .attr("r", 4.5)
        .style("fill", function (d) { return d._children ? "lightsteelblue" : "#fff"; }).html(function (d) { return "<title>" + retornaPercentual(d) + " (" + (100 * d.size / tamanhoTotal).toPrecision(3) + "% do total)</title>"; });

    function retornaPercentual(d) {
        if (d.parent == null)
            return "";

        var percentage = (100 * d.size / d.parent.size).toPrecision(3);
        var percentageString = percentage + "%";
        if (percentage < 0.1) {
            percentageString = "< 0.1%";
        }
        return percentageString;
    }

    nodeEnter.append("text")
        .attr("x", function (d) { return d.children || d._children ? -13 : 13; })
        .attr("dy", ".35em")
        .attr("text-anchor", function (d) { return d.children || d._children ? "end" : "start"; })
        .text(function (d) { return d.valor })
        .style("fill-opacity", 1e-6);

    // Transition nodes to their new position.
    var nodeUpdate = node.transition()
        .duration(duration)
        .attr("transform", function (d) { return "translate(" + d.x + "," + d.y + ")"; });

    nodeUpdate.select("circle")
        .attr("r", 4.5)
        .style("fill", function (d) { return d._children ? "lightsteelblue" : "#fff"; });

    nodeUpdate.select("text")
        .style("fill-opacity", 1);

    // Transition exiting nodes to the parent's new position.
    var nodeExit = node.exit().transition()
        .duration(duration)
        .attr("transform", function (d) { return "translate(" + source.y + "," + source.x + ")"; })
        .remove();

    nodeExit.select("circle")
        .attr("r", 1e-6);

    nodeExit.select("text")
        .style("fill-opacity", 1e-6);

    // Update the links…
    var link = svg.selectAll("path.link")
        .data(links, function (d) { return d.target.id; });

    // Enter any new links at the parent's previous position.
    link.enter().insert("path", "g")
        .attr("class", function (d) { return d.target.caminho === true ? "meuLink" : "link"; })
        .attr("d", function (d) {
            var o = { x: source.x0, y: source.y0 };
            return diagonal({ source: o, target: o });
        });

    // Transition links to their new position.
    link.transition()
        .duration(duration)
        .attr("d", diagonal);

    // Transition exiting nodes to the parent's new position.
    link.exit().transition()
        .duration(duration)
        .attr("d", function (d) {
            var o = { x: source.x, y: source.y };
            return diagonal({ source: o, target: o });
        })
        .remove();

    // Stash the old positions for transition.
    nodes.forEach(function (d) {
        d.x0 = d.x;
        d.y0 = d.y;
    });
}
