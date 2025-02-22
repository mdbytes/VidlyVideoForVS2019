﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Vidly.Models;
using Vidly.ViewModels;

namespace Vidly.Controllers
{
    
    public class CustomersController : Controller
    {
        private ApplicationDbContext _context;

        public CustomersController()
        {
            _context = new ApplicationDbContext();
        }

        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
        }


        public ViewResult Index()
        {
            var customers = _context.Customers.Include(c => c.MembershipType).ToList();

            var UserEmail = User.Identity.Name;

            var viewModel = new CustomerListViewModel
            {
                Customers = customers,
                UserEmail = UserEmail
            };

            return View(viewModel);
        }

        public ActionResult Details(int id)
        {
            var customer = _context.Customers.SingleOrDefault(c => c.Id == id);

            if (customer == null)
                return HttpNotFound();

            return View(customer);
        }

        public ActionResult New()
        {
            var membershipTypes = _context.MembershipTypes.ToList();
            var viewModel = new NewCustomerViewModel
            {
                MembershipTypes = membershipTypes

            };


            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Create(Customer customer)
        {


            if(customer.Id == 0)
            {
                if(customer.MembershipTypeId == 0)
                {
                    customer.MembershipTypeId = 1;
                }
                _context.Customers.Add(customer);
            } else
            {
                var customerInDb = _context.Customers.Single(c => c.Id == customer.Id);
                customerInDb.Name = customer.Name;
                customerInDb.Birthday = customer.Birthday;
                customerInDb.MembershipTypeId = customer.MembershipTypeId;
                customerInDb.IsSubscribedToNewsletter = customer.IsSubscribedToNewsletter;

            }
            
            _context.SaveChanges();

            return RedirectToAction("Index","Customers");
        }

        public ActionResult Edit(int id)
        {

            var customer = _context.Customers.SingleOrDefault(c => c.Id == id);

            if (customer == null)
                return HttpNotFound();

            var viewModel = new ExistingCustomerViewModel
            {
                Customer = customer,
                MembershipTypes = _context.MembershipTypes.ToList()
            };

            return View("Edit",viewModel);
        }

        public ActionResult Delete(int id)
        {
            var CustomerInDb = _context.Customers.SingleOrDefault(c => c.Id == id);

            _context.Customers.Remove(CustomerInDb);

            _context.SaveChanges();

            return RedirectToAction("Index", "Customers");

        }
    }
}