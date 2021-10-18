namespace Skelometry
{
    using System;
    // While a point is the simplest element, it also gives zero information as you can't tell
    // where it is without x,y,z information. There is also zero information about its size, weight, color etc.
    public class Point
    {
    }
}

/*
Element  // discrete object, can join multiple categories based on shared properties
Property // attribute of object that can be measured against others of same property category,
         // Simplest property is 'belongs to category', identity, can be counted but combining by this prop makes a new single object.
Category // elements that can share a common unit selected from themselves
Unit     // unit of measure, arbitrarily chosen from a category. When fully appended, each dimension has a unit, 
         // and a complex object has multiple joined units (like mph or 3d basis vectors).
Proportion // measuring something against the unit from a shared category. This is a partial operation. Can be length, area etc
Append   // place elements against each other, discrete objects count, properties add.
         // continuous objects combine into a single object and shared properties add
Combine  // units combine (multiply) by scaling to proportions. Identical units combine proportionally
         // causing the properties to scale proportionally. A new unit is chosen (based on op order?)
Arrange  // methods for arranging discrete objects (line, grid, cube, hexgrid, fibonacci, permutations etc)
Link     // method of joining discrete objects (like a link), allows converting discrete to continuous.
         // can be a line, but also proportional scale, parabolic scale etc.



*/