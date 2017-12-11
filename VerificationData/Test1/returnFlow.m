function flow = returnFlow(vol,time)
flow=zeros(size(vol));
A = vol(2:end)-vol(1:end-1);
B = time(2:end)-time(1:end-1);
flow(2:end) = A./B;
end
