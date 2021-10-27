#include "z3++.h"

using namespace z3;

// Z3 Example for checking de Morgan's Rule.
int main(int argc, char** argv){
  std::cout << "z3 example" << std::endl;

  context c;

  expr x =  c.bool_const("x");
  expr y = c.bool_const("y");
  expr conjecture = !(x && y) == (!x || !y);
  
  solver s(c);
  // adding the negation of the conjecture as a constraint.
  // You can add more than one assertion. They will be anded together
  s.add(!conjecture);
  std::cout << s << "\n";
  switch (s.check()) {
  case unsat:   std::cout << "de-Morgan is valid\n"; break;
  case sat:     std::cout << "de-Morgan is not valid\n"; break;
  case unknown: std::cout << "unknown\n"; break;
  }
}
