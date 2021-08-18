function [maxi] = one(y,z) 
x=-1:0.01:1;
f=-exp(-3.4.*x).*sign(sin(43.0.*x));
plot(x,f,y,z,'o');
grid on;
maxi=max(f);