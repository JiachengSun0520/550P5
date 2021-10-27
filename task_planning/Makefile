Z3_DIR = /usr
CXXFLAGS = -O2 # change to -g when debugging code
INCLUDE_FLAGS = -I${Z3}/include
LD_FLAGS = -L${Z3}/lib -lz3
CXX=g++

z3: z3_example.cpp
	$(CXX) $(CXXFLAGS) $(INCLUDE_FLAGS) -o z3_example z3_example.cpp $(LD_FLAGS)

clean:
	rm *.o z3_example
