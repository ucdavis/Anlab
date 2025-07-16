export function getOptions(useCoa: boolean) {
  return {
    UCB: {
      detail:
        "Loc(1) - Acct(5) - Fund(5) - Org(5) - Prog(2) - Proj(6) - Flex(5) ",
      example: "1-12345-12345-12345-44-A23456-A2345",
      name: "UCB",
    },
    UCSF: {
      detail:
        "Loc(1) - BusinessUnit(5) - Acct(5) - Fund(4) - DeptID(6) - Proj(7) - Activity(2) - Function(2) - Flex(6)",
      example: "2-A2345-12345-1234-123456-A234567-12-12-A23456",
      name: "UCSF",
    },
    UCD: {
      detail: useCoa
        ? "Use the Coa Picker to build a PPM or GL account. Note, the Expenditure Type/Natural Account may be overwritten."
        : "Loc(1) - Acct(7) / Subacct(5)",
      example: useCoa ? "SP00000001-000001-0000000-770003" : "3-ABCDE12/12345",
      name: "UCD",
    },
    UCLA: {
      detail:
        "Loc(1) - Acct(6) - CostCenter(2) - Fund(5) - Project(1-6) - Sub(2) - Object(4) - Src(1-6) ",
      example: "4-123456-A1-12345-123456-12-1234-A23456",
      name: "UCLA",
    },
    UCR: {
      detail:
        "Loc(1) - Acct(6) - Activity(6) - Fund(5) - Func(2) - CostCenter(4/5) - Proj(5)",
      example: "5-123456-A23456-12345-12-A2345-A2345",
      name: "UCR",
    },
    UCSD: {
      detail: "Loc(1) - Index(7) - Fund(5/6) - Org(6) - Acc(6) - Prog(6)",
      example: "6-A234567-A23456-123456-123456-123456",
      name: "UCSD",
    },
    UCSC: {
      detail:
        "Loc(1) - Fund(5) - Org(6) - Acct(6) - Prog(2) - Activity(1-6) - DocRef(1-6)",
      example: "7-12345-123456-A23456-12-A23456-A23456",
      name: "UCSC",
    },
    UCSB: {
      detail: "Loc(1) - Acct(6) - Fund(5) - Obj(4) - Sub(1) - Ref(1-6)",
      example: "8-123456-12345-1234-12-A23456",
      name: "UCSB",
    },
    UCI: {
      detail:
        "Loc(1) - Acct(7) - Fund(5) - Sub(2) - Obj(4) - Ref(1-10) - Src(1-6) - Proj(1-6)",
      example: "9-A234567-12345--12-A234-A234567890-A23456-A23456",
      name: "UCI",
    },
    UCM: {
      detail:
        "Location(1) - Entity(4) - Fund(5) - Financial Unit(7) - Account(6) - Function(2) - Program(3) - Project(10) - Physical Location(3) - Sub-Activity(6) - InterEntity(4) - Future 1(6) - Future 2(6)",
      example: "0-1234-12345-A123456-123456-12-ABC-ABCDEFGHIJ-123-ABCDEF-1234",
      name: "UCM",
    },
    MOP: {
      detail:
        "Loc(1) - Acct(6) - CostCenter(2) - Fund(5) - Proj(1-6) - Sub(2) - Obj(4) - Src(1-6)",
      example: "M-123456-A2-12345-123456-12-1234-A23456",
      name: "M-OP",
    },
  };
}

