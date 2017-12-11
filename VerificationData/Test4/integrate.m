function Y = integrate(time,flow)
Y = zeros(size(flow));
for i = 2:length(flow)
    Y(i) = Y(i-1) + flow(i-1)*(time(i)-time(i-1));
end