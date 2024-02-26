using System;
using System.Threading.Tasks;
using AutoMapper;
using AwesomeFruits.Application.DTOs;
using AwesomeFruits.Application.Interfaces;
using AwesomeFruits.Application.Utilities;
using AwesomeFruits.Domain.Entities;
using AwesomeFruits.Domain.Exceptions;
using AwesomeFruits.Domain.Interfaces;

namespace AwesomeFruits.Application.Services;

public class UserService : IUserService
{
    private readonly IMapper _mapper;
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository, IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<UserDto> GetValidLoggedInUser(LoginUserDto loginUserDto)
    {
        var getUser = await _userRepository.FindByUserNameAsync(loginUserDto.UserName);

        return getUser != null && PasswordHasher.VerifyPassword(getUser.PasswordHash, loginUserDto.Password,
            Convert.FromBase64String(getUser.PasswordSalt))
            ? _mapper.Map<UserDto>(getUser)
            : throw new UserCredentialsNotValidException();
    }

    public async Task<UserDto> SaveUserAsync(SaveUserDto saveUserDto)
    {
        var getUser = await _userRepository.FindByUserNameAsync(saveUserDto.UserName);

        if (getUser != null) throw new UserNameAlreadyExistsException();

        var (hashedPassword, salt) = PasswordHasher.HashPassword(saveUserDto.Password);

        var user = _mapper.Map<User>(saveUserDto);

        user.PasswordHash = hashedPassword;
        user.PasswordSalt = Convert.ToBase64String(salt);

        var saveUser = await _userRepository.SaveAsync(user);

        return _mapper.Map<UserDto>(saveUser);
    }
}