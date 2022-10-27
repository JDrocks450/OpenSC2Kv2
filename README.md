# Open SimCity 2000 (ver. 2)

![image](https://user-images.githubusercontent.com/16988651/198161327-b8ca8d6f-bd2a-474e-b08a-2bc56783a45e.png)

## Preface / References
This is a continuation of OpenSC2K - a project to reimplement SimCity 2000 in TypeScript.

You can find the original project here:

https://github.com/nicholas-ochoa/OpenSC2K

#### This project is by extension based on the following resources:

Based on the work of Dale Floer

SimCity 2000 saved city specification
MIF / LARGE.DAT graphics artwork specification https://github.com/dfloer/SC2k-docs

Based on the work of David Moews

SimCity 2000 for MS-DOS file format; unofficial partial information http://djm.cc/dmoews.html

Portions of the SC2 import logic are based on sc2kparser created by Objelisks and distributed under the terms of the ISC license.
https://github.com/Objelisks/sc2kparser

## What is this?

This project is SimCity 2000, reimplemented in C#. This project aims to preserve this older software title through reimplementing its engine in
modern langauges and open-source. This project is modular, with the API making up the bulk of the actual code powering the experience.

## Can I play it?

Not at the moment, refer to the roadmap for any updates in regards to gameplay.

## What's done?

* CityViewer program that interacts with API functions for *.SC2 loading, graphic resource importing, layout importing, positional calculations, etc.
* Graphic Resource viewer that interacts with the API in ways stated above.
