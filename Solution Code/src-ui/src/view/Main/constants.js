const modules = {
    variables: 1,
    displayTemplate: 2,
    products: 3,
    propertyUDP: 4,
  };

  const rowData = [
    {
      unit: "Unidad1",
      master: null,
      groups: [
        {
          group: "Variable Group 1",
          variables: [
            { id: 1, name: "Var001" },
            { id: 2, name: "Var002" },
            { id: 3, name: "Var003" }
          ]
        },
        {
          group: "Variable Group 2",
          variables: [
            { id: 4, name: "Var004" },
            { id: 5, name: "Var005" }
          ]
        }
      ]
    },
    {
      unit: "Unidad2",
      master: "Unidad1",
      groups: [
        {
          group: "Variable Group 5",
          variables: [
            { id: 6, name: "Var006" },
            { id: 7, name: "Var007" }
          ]
        },
        {
          group: "Variable Group 6",
          variables: [
            { id: 11, name: "Var011" },
            { id: 12, name: "Var012" }
          ]
        }
      ]
    },
    {
      unit: "Unidad3",
      master: null,
      groups: [
        {
          group: "Variable Group 3",
          variables: [
            { id: 8, name: "Var008" },
            { id: 9, name: "Var009" }
          ]
        }
      ]
    },
    {
      unit: "Unidad4",
      master: "Unidad3",
      groups: [
        {
          group: "Variable Group 4",
          variables: [
            { id: 10, name: "Var010" }
          ]
        }
      ]
    }
  ];
  
  

const stepsConfig = {
  [modules.variables]: [
    { title: 'Create Variable Details' },
    { title: 'Set UDPs & Extended Info' },
    { title: 'Attach to Display' },
    { title: 'Review' }
  ],
  [modules.displayTemplate]: [
    { title: 'Choose Template' },
    { title: 'Review Template' }
  ],
  [modules.products]: [
    { title: 'Add Product Details' },
    { title: 'Set Product Specifications' },
    { title: 'Review Products' }
  ],
  [modules.propertyUDP]: [
    { title: 'Define Properties' },
    { title: 'Assign UDPs' }
  ]
};

export { modules, rowData, stepsConfig };
