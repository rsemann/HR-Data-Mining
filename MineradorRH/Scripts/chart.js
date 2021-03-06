﻿// Dimensions of sunburst.
var width = 900;
var widthc = 1050;
var height = 600;
var radius = Math.min(width, height) / 2;

// Breadcrumb dimensions: width, height, spacing, width of tip/tail.
var b = { w: 220, h: 50, s: 3, t: 50 };

// make `colors` an ordinal scale
var colors = d3.scale.category20c();

// Total size of all segments; we set this later, after loading the data.
var totalSize = 0;

var vis = d3.select("#chart").append("svg:svg")
    .attr("width", width)
    .attr("height", height)
    .append("svg:g")
    .attr("id", "container")
    .attr("transform", "translate(" + width / 2 + "," + height / 2 + ")");

var partition = d3.layout.partition()
    .size([2 * Math.PI, radius * radius])
    .value(function (d) { return d.size; });

var arc = d3.svg.arc()
    .startAngle(function (d) { return d.x; })
    .endAngle(function (d) { return d.x + d.dx; })
    .innerRadius(function (d) { return Math.sqrt(d.y); })
    .outerRadius(function (d) { return Math.sqrt(d.y + d.dy); });



createVisualization();

// Main function to draw and set up the visualization, once we have the data.
function createVisualization() {
    $.ajax(
    {
        url: "/Home/BuscarGrafico",
        type: 'GET',
        cache: false,
        dataType: 'json',
        data: { id: $("#ArvoreGerada").val() },
        success: function (result) {
            var json = result;
            // Basic setup of page elements.
            initializeBreadcrumbTrail();

            // Bounding circle underneath the sunburst, to make it easier to detect
            // when the mouse leaves the parent g.
            vis.append("svg:circle")
                .attr("r", radius)
                .style("opacity", 0);

            // For efficiency, filter nodes to keep only those large enough to see.
            var nodes = partition.nodes(json)
                .filter(function (d) {
                    return (d.dx > 0.005); // 0.005 radians = 0.29 degrees
                });

            var path = vis.data([json]).selectAll("path")
                       .data(nodes)
                       .enter().append("svg:path")
                       .attr("display", function (d) { return d.depth ? null : "none"; })
                       .attr("d", arc)
                       .attr("fill-rule", "evenodd")
                       .style("fill", function (d) { return d.color; })
                       .style("opacity", 1)
                       .on("mouseover", mouseover)
                       .each(stash)
                        .transition()
                        .duration(750)
                            .attrTween("d", arcTween);

            d3.select("#container").on("mouseleave", mouseleave);

            totalSize = path.node().__data__.size;
        },
        error: function (error) {
        }
    });
};

function arcTween(a) {
    var i = d3.interpolate({ x: a.x0, dx: a.dx0 }, a);
    return function (t) {
        var b = i(t);
        a.x0 = b.x;
        a.dx0 = b.dx;
        return arc(b);
    };
};

function stash(d) {
    d.x0 = 0; // d.x;
    d.dx0 = 0; //d.dx;
};

// Fade all but the current sequence, and show it in the breadcrumb trail.
function mouseover(d) {
    var percentage = (100 * d.size / d.parent.size).toPrecision(3);
    var percentageString = percentage + "%";
    if (percentage < 0.1) {
        percentageString = "< 0.1%";
    }

    d3.select("#percentage")
        .text(d.valorClasseMeta);

    d3.select("#explanation")
        .style("visibility", "");

    var sequenceArray = getAncestors(d);
    updateBreadcrumbs(sequenceArray, percentageString + " (" + (100 * d.size / totalSize).toPrecision(3) + "% do total)");

    // Fade all the segments.
    d3.selectAll("path")
        .style("opacity", 0.3).html("<title>"+d.valorClasseMeta+"</title>");

    // Then highlight only those that are an ancestor of the current segment.
    vis.selectAll("path")
        .filter(function (node) {
            return (sequenceArray.indexOf(node) >= 0);
        })
        .style("opacity", 1);
}
d3.select(self.frameElement).style("height", height + "px");

// Restore everything to full opacity when moving off the visualization.
function mouseleave(d) {

    // Hide the breadcrumb trail
    d3.select("#trail")
        .style("visibility", "hidden");

    // Deactivate all segments during transition.
    d3.selectAll("path").on("mouseover", null);

    // Transition each segment to full opacity and then reactivate it.
    d3.selectAll("path")
        .transition()
        .duration(1000)
        .style("opacity", 1)
        .each("end", function () {
            d3.select(this).on("mouseover", mouseover);
        });

    d3.select("#explanation")
        .transition()
        .duration(1000)
        .style("visibility", "hidden");
}

// Given a node in a partition layout, return an array of all of its ancestor
// nodes, highest first, but excluding the root.
function getAncestors(node) {
    var path = [];
    var current = node;
    while (current.parent) {
        path.unshift(current);
        current = current.parent;
    }
    return path;
}

function initializeBreadcrumbTrail() {
    // Add the svg area.
    var trail = d3.select("#sequence").append("svg:svg")
        .attr("width", 2100)
        .attr("height", 50)
        .attr("id", "trail");
    // Add the label at the end, for the percentage.
    trail.append("svg:text")
      .attr("id", "endlabel")
      .style("fill", "#000");
}

// Generate a string that describes the points of a breadcrumb polygon.
function breadcrumbPoints(d, i) {
    var points = [];
    points.push("0,0");
    points.push(b.w + ",0");
    points.push(b.w + b.t + "," + (b.h));
    points.push(b.w + "," + b.h);
    points.push("0," + b.h);
    if (i > 0) { // Leftmost breadcrumb; don't include 6th vertex.
        points.push(b.t + "," + (b.h));
    }
    return points.join(" ");
}

// Update the breadcrumb trail to show the current sequence and percentage.
function updateBreadcrumbs(nodeArray, percentageString) {

    // Data join; key function combines name and depth (= position in sequence).
    var g = d3.select("#trail")
        .selectAll("g")
        .data(nodeArray, function (d) { return d.name + d.depth; });

    // Add breadcrumb and label for entering nodes.
    var entering = g.enter().append("svg:g");

    entering.append("svg:polygon")
        .attr("points", breadcrumbPoints)
        .style("fill", function (d) { return d.color; });

    var xPosicao;
    entering.append("svg:text")
        .attr("x", function () { xPosicao = b.t; return xPosicao; })
        .attr("y", 17)
        .attr("dy", "0.35em")
        .attr("text-anchor", "start")
        .html(function (d) { return "<tspan>" + d.legenda + "</tspan>" + (d.valor == null ? "" : "<tspan x='" + xPosicao + "' dy='1.5em'>" + d.valor) + "</tspan>"; });

    if (entering[0][0] != null)
        {
        entering[0][0].lastChild.attributes[0].value = 5;
        entering[0][0].lastChild.lastChild.attributes[0].value = 5;
    }

    // Set position for entering and updating nodes.
    g.attr("transform", function (d, i) {
        return "translate(" + i * (b.w + b.s) + ", 0)";
    });

    // Remove exiting nodes.
    g.exit().remove();

    // Now move and update the percentage at the end.
    d3.select("#trail").select("#endlabel")
        .attr("x", (nodeArray.length + 0.5) * (b.w + b.s))
        .attr("y", b.h / 2)
        .attr("dy", "0.35em")
        .attr("text-anchor", "middle")
        .text(percentageString);

    // Make the breadcrumb trail visible, if it's hidden.
    d3.select("#trail")
        .style("visibility", "");

}

