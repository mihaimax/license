﻿using Microsoft.EntityFrameworkCore;
using StudentPortal.Data;
using StudentPortal.Interfaces;
using StudentPortal.Models;

namespace StudentPortal.Repositories
{
    public class StudentRepository : IStudentRepository
    {
        private readonly ApplicationDbContext _context;
        public StudentRepository(ApplicationDbContext context) => _context = context;

        public async Task<Student?> GetByIdAsync(int studentId)
            => await _context.Students.FirstOrDefaultAsync(s => s.StudentId == studentId);

        public async Task<List<Student>> GetAllAsync()
            => await _context.Students.ToListAsync();

        public async Task<bool> AddAsync(Student student)
        {
            await _context.Students.AddAsync(student);
            return await _context.SaveChangesAsync() > 0;
        }
        public async Task<bool> ExistsAsync(int studentId)
            => await _context.Students.AnyAsync(s => s.StudentId == studentId);

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Student>> GetAllStudentsAsync()
            => await _context.Students.ToListAsync();

        public async Task<Student?> GetStudentByIdAsync(int id)
            => await _context.Students.FirstOrDefaultAsync(s => s.StudentId == id);

        public async Task<int> GetStudentIdByUserNameAsync(string userName)
        {
            var student = await _context.Students
                .FirstOrDefaultAsync(s => s.UserId == userName);
            return student?.StudentId ?? 0;
        }
        public async Task<string> GETPDFFileNameAsync(string userName)
        {
            var student = await _context.Students
                .FirstOrDefaultAsync(s => s.UserId == userName);
            return student?.Specialization + "_" + student?.Year + "_" + student?.Semester + ".pdf" ?? string.Empty;
        }
        public async Task<IEnumerable<Student>> GetAllStudentsForSituationAsync(string specialization, int year, int semester)
        {
            return await _context.Students
            .Include(s => s.User)
            .Where(s =>
                s.Specialization == specialization &&
                s.Year == year &&
                s.Semester == semester)
            .ToListAsync();
        }
    }
}
