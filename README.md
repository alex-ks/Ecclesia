# Ecclesia

Workflow management system

## Ecclesia.LocalExecutor

Simple [Executor API](https://app.swaggerhub.com/apis/alex-ks/ict-executor) implementation provided by ICT-Xored team of CompTech@NSK 2018 Winter school. Original sources can be found [here](https://github.com/alex-ks/comptech-nsk-ict). Thank you, brave souls!

## Filomena

F# compiler wich produces computation graph description in terms of [Executor API](https://app.swaggerhub.com/apis/alex-ks/ict-executor). Allows program to be executed as parallel as it can be. You can play with the Filomena [here](ccfit.nsu.ru/~komissarov/Filomena/) - please report errors and strange behaviour to its [GitHub issue tracker](https://github.com/alex-ks/Filomena/issues).

Currently supported features:
- Value bindings
- Tuples
- Arithmetic operations
- Function applications (if you can find some)
- Module opening (list of modules is not published yet)
